// <copyright file="CommentTagParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Dev.Imported;
using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// A parser for <see cref="CommentTag"/>
    /// </summary>
    public class CommentTagParser : Parser
    {
        private char tagId = '!';

        /// <summary>
        /// Tries to match a comment tag from the provided slice
        /// </summary>
        /// <param name="processor">The processor</param>
        /// <param name="slice">The slice</param>
        /// <returns>If a comment tag was matched</returns>
        public override bool Match(Processor processor, ref StringSlice slice)
        {
            var tagStart = slice.Start - processor.CurrentTags.StartTag.Length;
            var index = slice.Start;

            while (slice[index].IsWhitespace())
            {
                index++;
            }

            var match = slice[index];
            if (match == tagId)
            {
                slice.Start = index;
                var startIndex = index + 1;

                var commentTag = new CommentTag
                {
                    TagStartPosition = tagStart,
                    ContentStartPosition = startIndex,
                    IsClosed = false
                };
                processor.CurrentTag = commentTag;

                while (!slice.IsEmpty && !slice.Match(processor.CurrentTags.EndTag))
                {
                    slice.NextChar();
                }

                if (slice.IsEmpty)
                {
                    return false;
                }

                var contentEndIndex = slice.Start - 1;

                commentTag.TagEndPosition = slice.Start + processor.CurrentTags.EndTag.Length;
                commentTag.ContentEndPosition = slice.Start;
                commentTag.Content = new StringSlice(slice.Text, startIndex, contentEndIndex).ToString();
                commentTag.IsClosed = true;
                slice.Start += processor.CurrentTags.EndTag.Length;
                return true;
            }

            return false;
        }
    }
}