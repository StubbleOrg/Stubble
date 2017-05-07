// <copyright file="InlineToken{T}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Stubble.Core.Tokens
{
    /// <summary>
    /// A base class for a generic InlineTag
    /// </summary>
    /// <typeparam name="T">The type of the tag</typeparam>
    public abstract class InlineToken<T> : InlineToken, IEquatable<T>
    {
        /// <summary>
        /// Is the tag equal to the other tag
        /// </summary>
        /// <param name="other">The other tag</param>
        /// <returns>If they are equal</returns>
        public abstract bool Equals(T other);

        /// <summary>
        /// Is the tag equal to the object
        /// </summary>
        /// <param name="obj">the object</param>
        /// <returns>If they are equal</returns>
        public abstract override bool Equals(object obj);

        /// <summary>
        /// The hashcode for the tag
        /// </summary>
        /// <returns>The hashcode</returns>
        public abstract override int GetHashCode();
    }
}
