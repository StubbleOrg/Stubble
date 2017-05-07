// <copyright file="InstanceMustacheParser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Parser.Interfaces;
using Stubble.Core.Tokens;

namespace Stubble.Core.Parser
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
