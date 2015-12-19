// <copyright file="StubbleDataMissException.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Stubble.Core.Classes.Exceptions
{
    public class StubbleDataMissException : Exception
    {
        public StubbleDataMissException() { }
        public StubbleDataMissException(string message) : base(message) { }
        public StubbleDataMissException(string message, Exception innerException) : base(message, innerException) { }
    }
}
