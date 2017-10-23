// <copyright file="PartialLambdaExpressionDefinition.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq.Expressions;

namespace Stubble.Compilation.Renderers
{
    /// <summary>
    /// A simple class for holding variable assignments and lambda expressions
    /// </summary>
    public class PartialLambdaExpressionDefinition
    {
        /// <summary>
        /// Gets or sets the variable the partial will be assigned to
        /// </summary>
        public ParameterExpression Variable { get; set; }

        /// <summary>
        /// Gets or sets the lambda expression representing the partial
        /// </summary>
        public LambdaExpression Partial { get; set; }
    }
}
