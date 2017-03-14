// <copyright file="RendererTestsBase.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;

namespace Stubble.Core.Tests.Renderers
{
    public class RendererTestsBase : IDisposable
    {
        public RendererTestsBase()
        {
            MemStream = new MemoryStream();
            StreamWriter = new StreamWriter(MemStream);
        }

        public MemoryStream MemStream { get; }

        public StreamWriter StreamWriter { get; }

        public void Dispose()
        {
            MemStream.Dispose();
        }
    }
}
