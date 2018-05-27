// <copyright file="Processor.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Exceptions;
using Stubble.Core.Helpers;
using Stubble.Core.Imported;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Parser.TokenParsers;
using Stubble.Core.Tokens;

namespace Stubble.Core.Parser
{
    /// <summary>
    /// A processor for turning a StringSlice into tags
    /// </summary>
    public class Processor
    {
        private readonly List<InlineParser> inlineParsers;

        private readonly List<BlockParser> blockParsers;

        private readonly LiteralTagParser literalParser = new LiteralTagParser();

        private readonly List<MustacheToken> tagCache = new List<MustacheToken>();

        private bool firstOnLine = true;

        /// <summary>
        /// The content to be parsed and its state
        /// </summary>
        private StringSlice content;

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        /// <param name="inlineParsers">The inline parsers</param>
        /// <param name="blockParsers">The block parsers</param>
        public Processor(List<InlineParser> inlineParsers, List<BlockParser> blockParsers)
        {
            this.inlineParsers = inlineParsers;
            this.blockParsers = blockParsers;
        }

        /// <summary>
        /// Gets or sets the CurrentTags in scope
        /// </summary>
        public Classes.Tags CurrentTags { get; set; }

        /// <summary>
        /// Gets or sets the CurrentTag that was read
        /// </summary>
        public MustacheToken CurrentToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a tag has been seen on the current line
        /// </summary>
        public bool HasTagOnLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a non space characters has been seen on the line
        /// </summary>
        public bool HasSeenNonSpaceOnLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the indent for the current line
        /// </summary>
        public int LineIndent { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value for the default line indent
        /// </summary>
        public int DefaultLineIndent { get; set; }

        /// <summary>
        /// Gets or sets the slice to use to the default line indent
        /// </summary>
        public StringSlice DefaultLineIndentSlice { get; set; }

        /// <summary>
        /// Gets the document that has been parsed from the content string
        /// </summary>
        public MustacheTemplate Document { get; } = new MustacheTemplate();

        private Stack<BlockToken> OpenBlocks { get; } = new Stack<BlockToken>();

        /// <summary>
        /// Processes a slice and updates the Document to contain the parsed tags
        /// </summary>
        /// <param name="content">The slice to parse</param>
        public void ProcessTemplate(StringSlice content)
        {
            this.content = content;
            if (this.content.IsEmpty)
            {
                return;
            }

            while (this.content.CurrentChar != '\0')
            {
                if (this.content.Match(CurrentTags.StartTag))
                {
                    HasTagOnLine = true;
                    this.content.Start += CurrentTags.StartTag.Length;
                    var idx = this.content.Start;
                    while (this.content[idx].IsWhitespace())
                    {
                        idx++;
                    }

                    if (this.content[idx] == '/')
                    {
                        if (OpenBlocks.Count == 0)
                        {
                            var calculatedTagStart = this.content.Start - CurrentTags.StartTag.Length;
                            var startIndex = this.content.Start + 1;
                            while (!this.content.IsEmpty && !this.content.Match(CurrentTags.EndTag))
                            {
                                this.content.NextChar();
                            }

                            var closeBlockName = this.content.ToString(startIndex, this.content.Start).TrimEnd();

                            throw new StubbleException($"Unopened Block '{closeBlockName}' at {calculatedTagStart.ToString()}");
                        }

                        var openBlock = OpenBlocks.Peek();
                        var result = openBlock.Parser.TryClose(this, ref this.content, openBlock);
                        if (result)
                        {
                            OpenBlocks.Pop();
                            AddTag(CurrentToken);
                            continue;
                        }
                    }

                    if (TryOpenBlocks())
                    {
                        continue;
                    }

                    ProcessInlineTags();
                }
                else
                {
                    var result = literalParser.Match(this, ref this.content);
                    if (result != LiteralTagResult.NoContent)
                    {
                        AddTag(CurrentToken);
                    }

                    if (result == LiteralTagResult.NewLine)
                    {
                        NewLine();
                    }
                }
            }

            RemoveEmptyTags();

            ClearTagCache();

            if (OpenBlocks.Count > 0)
            {
                var block = OpenBlocks.Pop();
                throw new StubbleException($"Unclosed Block '{block.Identifier}' at {this.content.Start.ToString()}");
            }

            SquashAndNestTokens();
            SetTagContent();
        }

        private void SquashAndNestTokens()
        {
            var openblocks = new Stack<BlockToken>();
            for (var i = 0; i < Document.Children.Count; i++)
            {
                var tag = Document.Children[i];

                if (tag is LiteralToken && i + 1 < Document.Children.Count)
                {
                    var literalTag = tag as LiteralToken;

                    if (Document.Children[i + 1] is LiteralToken nextTag)
                    {
                        literalTag.ContentEndPosition = nextTag.ContentEndPosition;
                        literalTag.Indent = nextTag.Indent > literalTag.Indent ? nextTag.Indent : literalTag.Indent;
                        Document.Children.Remove(nextTag);
                        i--;
                        continue;
                    }
                }
                else if (tag is BlockCloseToken)
                {
                    var currentBlock = openblocks.Peek();
                    currentBlock?.Parser.EndBlock(this, currentBlock, tag as BlockCloseToken, content);
                    openblocks.Pop();
                    Document.Children.Remove(tag);
                    i--;
                    continue;
                }

                if (openblocks.Count > 0)
                {
                    var currentBlock = openblocks.Peek();
                    currentBlock.Children.Add(tag);
                    Document.Children.Remove(tag);
                    i--;
                }

                if (tag is BlockToken)
                {
                    openblocks.Push(tag as BlockToken);
                }
            }
        }

        private void SetTagContent()
        {
            for (var i = 0; i < Document.Children.Count; i++)
            {
                ProcessTag(Document.Children[i]);
            }

            void ProcessTag(MustacheToken tag)
            {
                switch (tag)
                {
                    case InlineToken inline:
                        inline.Content = new StringSlice(
                            content.Text,
                            inline.ContentStartPosition,
                            inline.ContentEndPosition - 1);
                        break;
                    case LiteralToken literal:
                        literal.Content = SliceHelpers.SplitSliceToLines(new StringSlice(
                            content.Text,
                            literal.ContentStartPosition,
                            literal.ContentEndPosition - 1)).ToArray();
                        break;
                    case BlockToken blockTag:
                        foreach (var child in blockTag.Children)
                        {
                            ProcessTag(child);
                        }

                        break;
                }
            }
        }

        private bool TryOpenBlocks()
        {
            foreach (var blockParser in blockParsers)
            {
                if (blockParser.TryOpenBlock(this, ref content) == ParserState.Break)
                {
                    OpenBlocks.Push(CurrentToken as BlockToken);
                    AddTag(CurrentToken);
                    return true;
                }
            }

            return false;
        }

        private void ProcessInlineTags()
        {
            foreach (var parser in inlineParsers)
            {
                if (parser.Match(this, ref content))
                {
                    AddTag(CurrentToken);
                    break;
                }
            }
        }

        private void AddTag(MustacheToken token)
        {
            if (token is INonSpace)
            {
                HasSeenNonSpaceOnLine = true;
            }

            if (firstOnLine)
            {
                AddIndentToTag(token);
                firstOnLine = false;
            }

            tagCache.Add(token);
        }

        private void NewLine()
        {
            RemoveEmptyTags();

            if (content.CurrentChar == '\n')
            {
                content.Start += 1;
            }
            else
            {
                content.Start += 2;
            }

            HasTagOnLine = false;
            HasSeenNonSpaceOnLine = false;
            firstOnLine = true;
            LineIndent = DefaultLineIndent;
            ClearTagCache();
        }

        private void RemoveEmptyTags()
        {
            if (HasTagOnLine && !HasSeenNonSpaceOnLine)
            {
                for (var i = 0; i < tagCache.Count; i++)
                {
                    var tag = tagCache[i];
                    if (tag is LiteralToken && ((LiteralToken)tag).IsWhitespace)
                    {
                        tagCache.Remove(tag);
                        i--;
                    }
                }
            }
        }

        private void ClearTagCache()
        {
            Document.Children.AddRange(tagCache);
            tagCache.Clear();
        }

        private void AddIndentToTag(MustacheToken token)
        {
            if (DefaultLineIndent > 0)
            {
                token.Indent = DefaultLineIndent;
            }
        }
    }
}
