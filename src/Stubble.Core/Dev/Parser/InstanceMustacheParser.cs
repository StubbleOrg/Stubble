// <copyright file="InstanceMustacheParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Dev.Tags;

namespace Stubble.Core.Dev.Parser
{
    /// <summary>
    /// Represents wrapper to the static mustache parse call
    /// </summary>
    public class InstanceMustacheParser : IMustacheParser
    {
        /// <inheritdoc/>
        public MustacheTemplate Parse(string text, Classes.Tags startingTags = null, int lineIndent = 0, ParserPipeline pipeline = null)
        {
            return MustacheParser.Parse(text, startingTags, lineIndent, pipeline);
        }
    }
}
