// <copyright file="EnumerationConverter.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Linq.Expressions;

namespace Stubble.Compilation.Class
{
    /// <summary>
    /// A class for holding the conversion to an IEnumerable with its inner type
    /// </summary>
    public class EnumerationConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerationConverter"/> class.
        /// </summary>
        /// <param name="converterExpression">An expression for converting to IEnumerable</param>
        /// <param name="childType">The inner type of the IEnumerable</param>
        public EnumerationConverter(
            Expression<Func<object, IEnumerable>> converterExpression,
            Type childType)
        {
            ConverterExpression = converterExpression;
            ChildType = childType;
        }

        /// <summary>
        /// Gets the converter to IEnumerable
        /// </summary>
        public Expression<Func<object, IEnumerable>> ConverterExpression { get; }

        /// <summary>
        /// Gets the child type to convert to
        /// </summary>
        public Type ChildType { get; }
    }
}
