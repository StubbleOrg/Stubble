// <copyright file="StubbleVisitorBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Builders
{
    /// <summary>
    /// A stubble builder that returns a <see cref="StubbleVisitorRenderer"/>
    /// </summary>
    public class StubbleVisitorBuilder : StubbleBuilder<StubbleVisitorRenderer>
    {
        /// <summary>
        /// Builds a new renderer from the builder settings
        /// </summary>
        /// <returns>A new stubble renderer</returns>
        public override StubbleVisitorRenderer Build()
        {
            var rendererSettings = BuildSettings();

            return new StubbleVisitorRenderer(rendererSettings);
        }
    }
}
