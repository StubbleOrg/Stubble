// <copyright file="RegistryResult.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stubble.Compilation.Contexts
{
    /// <summary>
    /// A result of a lookup from the registry
    /// </summary>
    public struct RegistryResult
    {
        /// <summary>
        /// Gets or sets the type associated to the expression
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the expression looked up from the registry
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// Checks if the object is equal to the passed object
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>If the object is equals to the current registry result</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is RegistryResult))
            {
                return false;
            }

            var result = (RegistryResult)obj;
            return EqualityComparer<Type>.Default.Equals(Type, result.Type) &&
                   EqualityComparer<Expression>.Default.Equals(Expression, result.Expression);
        }

        /// <summary>
        /// Gets the hashcode for the registry result
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            var hashCode = 1705332608;
            hashCode = (hashCode * -1521134295) + base.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<Type>.Default.GetHashCode(Type);
            hashCode = (hashCode * -1521134295) + EqualityComparer<Expression>.Default.GetHashCode(Expression);
            return hashCode;
        }
    }
}
