// <copyright file="BaseContext.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Interfaces;

namespace Stubble.Core.Contexts
{
    /// <summary>
    /// Represents the abstract base of any renderer context
    /// </summary>
    /// <typeparam name="TContext">The type of the actual context</typeparam>
    public abstract class BaseContext<TContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseContext{TContext}"/> class.
        /// </summary>
        /// <param name="partialLoader">A reference to loader for partials</param>
        /// <param name="parentContext">The parent context for the new context</param>
        public BaseContext(IStubbleLoader partialLoader, TContext parentContext)
        {
            ParentContext = parentContext;
            PartialLoader = partialLoader;
        }

        /// <summary>
        /// Gets the parent context of the current context
        /// </summary>
        public TContext ParentContext { get; }

        /// <summary>
        /// Gets the partial loader for the context
        /// </summary>
        public IStubbleLoader PartialLoader { get; }

        /// <summary>
        /// Returns a new <see cref="BaseContext{TContext}"/> with the given view and it's
        /// parent set as the current context
        /// </summary>
        /// <param name="newView">The data view to create the new context with</param>
        /// <returns>A new child data context of the current context</returns>
        public abstract TContext Push(object newView);
    }
}
