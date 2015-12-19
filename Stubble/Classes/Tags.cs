// <copyright file="Tags.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes.Exceptions;

namespace Stubble.Core.Classes
{
    public class Tags
    {
        public Tags(string startTag, string endTag)
        {
            StartTag = startTag;
            EndTag = endTag;
        }

        public Tags(string[] tags)
        {
            if (tags.Length != 2)
            {
                throw new StubbleException("Invalid Tags");
            }

            StartTag = tags[0];
            EndTag = tags[1];
        }

        public string StartTag { get; private set; }

        public string EndTag { get; private set; }

        public override string ToString()
        {
            return StartTag + " " + EndTag;
        }
    }
}
