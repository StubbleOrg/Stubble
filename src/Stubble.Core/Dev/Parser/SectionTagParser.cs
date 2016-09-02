// <copyright file="SectionTagParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Dev.Imported;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
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
                slice.Start = index;

                // Skip whitespace
                while (slice.CurrentChar.IsWhitespace())
                {
                    slice.NextChar();
                }

                var startIndex = slice.Start + 1;

                // Take characters until closing tag
                while (!slice.IsEmpty && !slice.Match(processor.CurrentTags.EndTag))
                {
                    slice.NextChar();
                }

                var sectionName = slice.ToString(startIndex, slice.Start).TrimEnd();

                var sectionTag = new SectionTag
                {
                    SectionName = sectionName,
                    StartPosition = tagStart,
                    Parser = this,
                    IsClosed = false
                };

                processor.CurrentTag = sectionTag;

                slice.Start += processor.CurrentTags.EndTag.Length;

                return ParserState.Break;
            }

            return ParserState.Continue;
        }

        /// <summary>
        /// Close the block with the given block close tag
        /// </summary>
        /// <param name="tag">The open tag</param>
        /// <param name="closeTag">the closing tag</param>
        public override void EndBlock(BlockTag tag, BlockCloseTag closeTag)
        {
            var sectionTag = tag as SectionTag;
            var sectionEndTag = closeTag as SectionEndTag;
            if (sectionTag != null && sectionEndTag != null)
            {
                if (sectionTag.SectionName.Equals(sectionEndTag.SectionName))
                {
                    sectionTag.EndPosition = sectionEndTag.EndPosition;
                    sectionTag.IsClosed = true;
                }
            }
        }

        /// <summary>
        /// Try to create a block close tag
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">the slice</param>
        /// <param name="tag">the current block tag</param>
        /// <returns>If the close was successful</returns>
        public override bool TryClose(Processor processor, ref StringSlice slice, BlockTag tag)
        {
            var sectionTag = (SectionTag)tag;
            while (slice.CurrentChar.IsWhitespace())
            {
                slice.NextChar();
            }

            var blockStart = slice.Start - processor.CurrentTags.StartTag.Length;
            var startIndex = slice.Start + 1; // Skip the slash

            // Take characters until closing tag
            while (!slice.IsEmpty && !slice.Match(processor.CurrentTags.EndTag))
            {
                slice.NextChar();
            }

            var sectionName = slice.ToString(startIndex, slice.Start).TrimEnd();
            if (sectionTag.SectionName == sectionName)
            {
                var tagEnd = slice.Start + processor.CurrentTags.EndTag.Length;

                var endTag = new SectionEndTag
                {
                    SectionName = sectionName,
                    EndPosition = tagEnd,
                    IsClosed = true
                };

                processor.CurrentTag = endTag;

                slice.Start += processor.CurrentTags.EndTag.Length;
                return true;
            }

            throw new StubbleException($"Cannot close Block '{sectionName}' at {blockStart}. There is already an unclosed Block '{sectionTag.SectionName}'");
        }
    }
}
