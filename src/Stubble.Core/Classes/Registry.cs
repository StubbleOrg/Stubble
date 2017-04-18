// <copyright file="Registry.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Stubble.Core.Classes.Tokens;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes
{
    /// <summary>
    /// A class holding the instance data for a Stubble Renderer
    /// </summary>
    public sealed class Registry
    {
        private static readonly HashSet<string> DefaultTokenTypes = new HashSet<string> { @"\/", "=", @"\{", "!" };
        private static readonly HashSet<string> ReservedTokens = new HashSet<string> { "name", "text" }; // Name and text are used internally for tokens so must exist}

        /// <summary>
        /// Initializes a new instance of the <see cref="Registry"/> class.
        /// </summary>
        public Registry()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Registry"/> class
        /// with given <see cref="RegistrySettings"/>
        /// </summary>
        /// <param name="tokenGetters">the token getters</param>
        public Registry(IDictionary<string, Func<string, Tags, ParserOutput>> tokenGetters)
        {
            SetTokenGetters(tokenGetters);
            SetTokenMatchRegex();
        }

        /// <summary>
        /// Gets a readonly dictionary of Token Getter functions
        /// </summary>
        public IReadOnlyDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters { get; private set; }

        /// <summary>
        /// Gets the generated Token match regex
        /// </summary>
        public Regex TokenMatchRegex { get; private set; }

        private void SetTokenGetters(IDictionary<string, Func<string, Tags, ParserOutput>> tokenGetters)
        {
            if (tokenGetters != null)
            {
                var mergedGetters = RegistryDefaults.DefaultTokenGetters.MergeLeft(tokenGetters);

                TokenGetters = new ReadOnlyDictionary<string, Func<string, Tags, ParserOutput>>(mergedGetters);
            }
            else
            {
                TokenGetters = new ReadOnlyDictionary<string, Func<string, Tags, ParserOutput>>(RegistryDefaults.DefaultTokenGetters);
            }
        }

        private void SetTokenMatchRegex()
        {
            var str = new HashSet<string>();
            foreach (var getter in TokenGetters)
            {
                if (ReservedTokens.Contains(getter.Key))
                {
                    continue;
                }

                str.Add(Parser.EscapeRegexExpression(getter.Key));
            }

            str.UnionWith(DefaultTokenTypes);

            TokenMatchRegex = new Regex(string.Join("|", str));
        }

        private static class RegistryDefaults
        {
            public static readonly IDictionary<string, Func<string, Tags, ParserOutput>> DefaultTokenGetters = new Dictionary
                <string, Func<string, Tags, ParserOutput>>
            {
                { "#", (s, tags) => new SectionToken(tags) { TokenType = s } },
                { "^", (s, tags) => new InvertedToken { TokenType = s } },
                { ">", (s, tags) => new PartialToken { TokenType = s } },
                { "&", (s, tags) => new UnescapedValueToken { TokenType = s } },
                { "name", (s, tags) => new EscapedValueToken { TokenType = s } },
                { "text", (s, tags) => new RawValueToken { TokenType = s } }
            };
        }
    }
}
