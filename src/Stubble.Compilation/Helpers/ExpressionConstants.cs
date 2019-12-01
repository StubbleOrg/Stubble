// <copyright file="ExpressionConstants.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq.Expressions;

namespace Stubble.Compilation.Helpers
{
    /// <summary>
    /// A class containing constants for use in expression trees
    /// </summary>
    internal static class ExpressionConstants
    {
        /// <summary>
        /// Gets the constant expression for true
        /// </summary>
        public static readonly ConstantExpression TrueConstant = Expression.Constant(true);

        /// <summary>
        /// Gets the constant expression for false
        /// </summary>
        public static readonly ConstantExpression FalseConstant = Expression.Constant(false);
    }
}
