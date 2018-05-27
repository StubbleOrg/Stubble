// <copyright file="SectionTagParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Exceptions;
using Stubble.Core.Imported;
using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Tokens;

namespace Stubble.Core.Parser.TokenParsers
{
    /// <summary>
    /// A parser for section tags
    /// </summary>
    public class SectionTagParser : BlockParser
    {
        private const char OpeningTagDelimiter = '#';

        /// <summary>
        /// Tries to open a section tag using the slice
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">The slice</param>
        /// <returns>The result of the match</returns>
        public override ParserState TryOpenBlock(Processor processor, ref StringSlice slice)
        {
            var tagStart = slice.Start - processor.CurrentTags.StartTag.Length;
            var index = slice.Start;

            while (slice[index].IsWhitespace())
            {
                index++;
            }

            var match = slice[index];
            if (match == OpeningTagDelimiter)
            {
                slice.Start = index + 1; // Skip delimiter

                while (slice.CurrentChar.IsWhitespace())
                {
                    slice.NextChar();
                }

                var startIndex = slice.Start;

                // Take characters until closing tag
                while (!slice.IsEmpty && !slice.Match(processor.CurrentTags.EndTag))
                {
                    slice.NextChar();
                }

                var sectionName = slice.ToString(startIndex, slice.Start).TrimEnd();
                var contentStartPosition = slice.Start + processor.CurrentTags.EndTag.Length;

                var sectionTag = new SectionToken
                {
                    SectionName = sectionName,
                    StartPosition = tagStart,
                    ContentStartPosition = contentStartPosition,
                    Parser = this,
                    IsClosed = false
                };

                processor.CurrentToken = sectionTag;

                slice.Start = contentStartPosition;

                return ParserState.Break;
            }

            return ParserState.Continue;
        }

        /// <summary>
        /// Close the block with the given block close tag
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="token">The open tag</param>
        /// <param name="closeToken">the closing tag</param>
        /// <param name="content">the content the tags were parsed from</param>
        public override void EndBlock(Processor processor, BlockToken token, BlockCloseToken closeToken, StringSlice content)
        {
            var sectionEndTag = closeToken as SectionEndToken;
            if (token is SectionToken sectionTag && sectionEndTag != null)
            {
                if (sectionTag.SectionName.Equals(sectionEndTag.SectionName))
                {
                    sectionTag.Tags = processor.CurrentTags;
                    sectionTag.EndPosition = sectionEndTag.EndPosition;
                    sectionTag.ContentEndPosition = sectionEndTag.ContentEndPosition;
                    sectionTag.IsClosed = true;
                    sectionTag.SectionContent = new StringSlice(content.Text, sectionTag.ContentStartPosition, sectionTag.ContentEndPosition - 1);
                }
            }
        }

        /// <summary>
        /// Try to create a block close tag
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">the slice</param>
        /// <param name="token">the current block tag</param>
        /// <returns>If the close was successful</returns>
        public override bool TryClose(Processor processor, ref StringSlice slice, BlockToken token)
        {
            var sectionTag = (SectionToken)token;
            while (slice.CurrentChar.IsWhitespace())
            {
                slice.NextChar();
            }

            var blockStart = slice.Start - processor.CurrentTags.StartTag.Length;
            slice.Start = slice.Start + 1; // Skip the slash

            while (slice.CurrentChar.IsWhitespace())
            {
                slice.NextChar();
            }

            var startIndex = slice.Start;

            // Take characters until closing tag
            while (!slice.IsEmpty && !slice.Match(processor.CurrentTags.EndTag))
            {
                slice.NextChar();
            }

            var sectionName = slice.ToString(startIndex, slice.Start).TrimEnd();
            if (sectionTag.SectionName == sectionName)
            {
                var tagEnd = slice.Start + processor.CurrentTags.EndTag.Length;

                var endTag = new SectionEndToken
                {
                    SectionName = sectionName,
                    EndPosition = tagEnd,
                    ContentEndPosition = blockStart,
                    IsClosed = true
                };

                processor.CurrentToken = endTag;

                slice.Start += processor.CurrentTags.EndTag.Length;
                return true;
            }

            throw new StubbleException($"Cannot close Block '{sectionName}' at {blockStart}. There is already an unclosed Block '{sectionTag.SectionName}'");
        }
    }
}
