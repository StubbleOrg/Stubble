// <copyright file="InterpolationTagParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Exceptions;
using Stubble.Core.Imported;
using Stubble.Core.Tokens;

namespace Stubble.Core.Parser.TokenParsers
{
    /// <summary>
    /// A parser for Interpolation Tags
    /// </summary>
    public class InterpolationTagParser : Interfaces.InlineParser
    {
        /// <summary>
        /// Tries to match interpolation tags from the current slice
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">The slice</param>
        /// <returns>If the match was successful</returns>
        public override bool Match(Processor processor, ref StringSlice slice)
        {
            var tagStart = slice.Start - processor.CurrentTags.StartTag.Length;
            var index = slice.Start;
            var escapeResult = true;
            var isTripleMustache = false;

            while (slice[index].IsWhitespace())
            {
                index++;
            }

            var match = slice[index];
            if (match == '&')
            {
                escapeResult = false;
                index++;
            }
            else if (match == '{')
            {
                escapeResult = false;
                isTripleMustache = true;
                index++;
            }

            while (slice[index].IsWhitespace())
            {
                index++;
            }

            slice.Start = index;
            var startIndex = index;

            var endTag = isTripleMustache ? '}' + processor.CurrentTags.EndTag : processor.CurrentTags.EndTag;
            while (!slice.IsEmpty && !slice.Match(endTag))
            {
                slice.NextChar();
            }

            var content = new StringSlice(slice.Text, startIndex, slice.Start - 1);
            content.TrimEnd();
            var contentEnd = content.End + 1;

            var tag = new InterpolationToken
            {
                EscapeResult = escapeResult,
                TagStartPosition = tagStart,
                ContentStartPosition = startIndex,
                IsClosed = true
            };

            if (!slice.Match(endTag))
            {
                throw new StubbleException($"Unclosed Tag at {slice.Start.ToString()}");
            }

            tag.ContentEndPosition = contentEnd;
            tag.TagEndPosition = slice.Start + endTag.Length;
            slice.Start += endTag.Length;

            processor.CurrentToken = tag;
            processor.HasSeenNonSpaceOnLine = true;

            return true;
        }
    }
}
