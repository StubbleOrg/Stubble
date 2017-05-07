// <copyright file="BlockToken{T}.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Stubble.Core.Tokens
{
    /// <summary>
    /// The base class for a generic BlockTag
    /// </summary>
    /// <typeparam name="T">The block tag type</typeparam>
    public abstract class BlockToken<T> : BlockToken, IEquatable<T>
    {
        /// <summary>
        /// Gets or sets the tags used to parse the block
        /// </summary>
        public Classes.Tags Tags { get; set; }

        /// <summary>
        /// Is the block tag equal to another
        /// </summary>
        /// <param name="other">The other</param>
        /// <returns>If the blocks are equal</returns>
        public abstract bool Equals(T other);

        /// <summary>
        /// Is the block tag equal to another object
        /// </summary>
        /// <param name="obj">The other object</param>
        /// <returns>If the object is equal to the block</returns>
        public abstract override bool Equals(object obj);

        /// <summary>
        /// Gets the hashcode for the block
        /// </summary>
        /// <returns>The hash code</returns>
        public new virtual int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                foreach (var item in Children)
                {
                    hash = (13 * hash) + item.GetHashCode();
                }

                hash = (13 * hash) + IsClosed.GetHashCode();
                return hash;
            }
        }
    }
}
