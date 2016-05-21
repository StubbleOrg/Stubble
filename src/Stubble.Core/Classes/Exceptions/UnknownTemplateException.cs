// <copyright file="UnknownTemplateException.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Stubble.Core.Classes.Exceptions
{
    /// <summary>
    /// Represents an exception coming from discovering a template using a <see cref="Interfaces.IStubbleLoader"/>
    /// </summary>
    public class UnknownTemplateException : StubbleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownTemplateException"/> class.
        /// </summary>
        public UnknownTemplateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownTemplateException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnknownTemplateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownTemplateException"/> class with
        /// a specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public UnknownTemplateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
