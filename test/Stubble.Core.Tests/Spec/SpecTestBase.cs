// <copyright file="SpecTestBase.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections;
using Xunit;
using Xunit.Abstractions;
using Stubble.Core.Dev;

namespace Stubble.Core.Tests.Spec
{
    public class SpecTestBase
    {
        internal readonly ITestOutputHelper OutputStream;

        public SpecTestBase(ITestOutputHelper output)
        {
            OutputStream = output;
        }

        public void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            var stubble = new StubbleStringRenderer();
            var output = data.Partials != null ? stubble.Render(data.Template, data.Data, data.Partials) : stubble.Render(data.Template, data.Data);

            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.Expected, output);
            Assert.Equal(data.Expected, output);
        }

        public void It_Can_Pass_Spec_Tests_Visitor(SpecTest data)
        {
            OutputStream.WriteLine(data.Name);
            var stubble = new StubbleVisitorRenderer();
            var output = data.Partials != null ? stubble.Render(data.Template, data.Data, data.Partials) : stubble.Render(data.Template, data.Data);

            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.Expected, output);
            Assert.Equal(data.Expected, output);
        }
    }
}
