// <copyright file="StubbleBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Classes;

namespace Stubble.Core
{
    /// <summary>
    /// Represents the primary builder class for creating <see cref="StubbleStringRenderer"/> instances
    /// </summary>
    public sealed class StubbleBuilder : StubbleBuilder<StubbleStringRenderer>
    {
        /// <summary>
        /// Gets the internal map of Token Getters
        /// </summary>
        internal IDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters { get; private set; } =
            new Dictionary<string, Func<string, Tags, ParserOutput>>();

        /// <summary>
        /// Adds a given type and token getter function to the Token Getters
        /// </summary>
        /// <param name="tokenType">The token type to add the token getter function for</param>
        /// <param name="tokenGetter">A token getter function</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public StubbleBuilder<StubbleStringRenderer> AddTokenGetter(string tokenType, Func<string, Tags, ParserOutput> tokenGetter)
        {
            return AddTokenGetter(new KeyValuePair<string, Func<string, Tags, ParserOutput>>(tokenType, tokenGetter));
        }

        /// <summary>
        /// Adds a given token getter to the Token Getters
        /// </summary>
        /// <param name="tokenGetter">A token getter</param>
        /// <returns>The IStubbleBuilder for chaining</returns>
        public StubbleBuilder<StubbleStringRenderer> AddTokenGetter(KeyValuePair<string, Func<string, Tags, ParserOutput>> tokenGetter)
        {
            TokenGetters.Add(tokenGetter);
            return this;
        }

        /// <summary>
        /// Builds a <see cref="StubbleStringRenderer"/> instance with the initalised settings
        /// </summary>
        /// <returns>Returns a <see cref="StubbleStringRenderer"/> with the initalised settings</returns>
        public override StubbleStringRenderer Build()
        {
            var rendererSettings = BuildSettings();

            return new StubbleStringRenderer(rendererSettings, new Registry(TokenGetters));
        }
    }
}
