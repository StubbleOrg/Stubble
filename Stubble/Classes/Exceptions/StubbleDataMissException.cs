// <copyright file="StubbleDataMissException.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Stubble.Core.Classes.Exceptions
{
    /// <summary>
    /// Represents errors that occur when data is not found when rendering a template
    /// </summary>
    public class StubbleDataMissException : StubbleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleDataMissException"/> class.
        /// </summary>
        public StubbleDataMissException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleDataMissException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StubbleDataMissException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleDataMissException"/> class with
        /// a specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public StubbleDataMissException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
