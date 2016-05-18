// <copyright file="StubbleException.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Stubble.Core.Classes.Exceptions
{
    /// <summary>
    /// Represents a general error coming from Stubble
    /// </summary>
    public class StubbleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleException"/> class.
        /// </summary>
        public StubbleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StubbleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleException"/> class with
        /// a specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public StubbleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
