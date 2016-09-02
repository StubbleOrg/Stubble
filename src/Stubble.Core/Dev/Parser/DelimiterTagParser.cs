// <copyright file="DelimiterTagParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Dev.Imported;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// A parser for Delimiter Tags
    /// </summary>
    public class DelimiterTagParser : Parser
    {
        private char[] openingTagDelimiter = { '=' };
        private char[] closingTagDelimiter = { '=' };

        /// <summary>
        /// Tries to match delimiter tags from the current slice
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">The slice</param>
        /// <returns>If the match was successful</returns>
        public override bool Match(Processor processor, ref StringSlice slice)
        {
            var tagStart = slice.Start - processor.CurrentTags.StartTag.Length;
            var index = slice.Start;

            while (slice[index].IsWhitespace())
            {
                index++;
            }

            var match = slice[index];
            if (match == openingTagDelimiter[0])
            {
                index++;
                while (slice[index].IsWhitespace())
                {
                    index++;
                }

                slice.Start = index;
                var startIndex = slice.Start;

                // Take Characters that aren't whitespace
                while (!slice.CurrentChar.IsWhitespace())
                {
                    slice.NextChar();
                }

                var startTag = slice.ToString(startIndex, slice.Start);

                // Skip Whitespace any
                while (slice.CurrentChar.IsWhitespace())
                {
                    slice.NextChar();
                }

                var endTagStartIndex = slice.Start;

                // Take characters until end delimiter;
                var closingTag = closingTagDelimiter[0] + processor.CurrentTags.EndTag;
                while (!slice.IsEmpty && !slice.Match(closingTag))
                {
                    slice.NextChar();
                }

                var endTag = new StringSlice(slice.Text, endTagStartIndex, slice.Start - 1);
                endTag.TrimEnd();
                var contentEnd = endTag.End + 1;

                var tag = new DelimiterTag
                {
                    TagStartPosition = tagStart,
                    ContentStartPosition = startIndex,
                    ContentEndPosition = contentEnd,
                    TagEndPosition = slice.Start + closingTag.Length,
                    StartTag = startTag,
                    EndTag = endTag.ToString(),
                    IsClosed = true
                };

                processor.CurrentTag = tag;

                processor.CurrentTags = new Classes.Tags(tag.StartTag, tag.EndTag);
                slice.Start += closingTag.Length;

                return true;
            }

            return false;
        }
    }
}
