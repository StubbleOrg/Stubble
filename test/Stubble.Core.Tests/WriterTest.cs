// <copyright file="WriterTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Stubble.Core.Classes;
using Stubble.Core.Tests.Fixtures;
using Xunit;

namespace Stubble.Core.Tests
{
    [CollectionDefinition("WriterCollection")]
    public class WriterCollection : ICollectionFixture<WriterTestFixture> { }

    [Collection("WriterCollection")]
    public class WriterTest
    {
        public Writer Writer;

        public WriterTest(WriterTestFixture fixture)
        {
            Writer = fixture.Writer;
        }

        [Fact]
        public void It_Can_Render_Templates()
        {
            var output = Writer.Render("{{foo}}", new { foo = "Bar" }, null, RenderSettings.GetDefaultRenderSettings());
            Assert.Equal("Bar", output);
        }

        [Fact]
        public void It_Treats_Strings_As_StringContent_Not_IEnumerable()
        {
            var output = Writer.Render("{{#foo}}{{foo}}{{/foo}}", new { foo = "Bar" }, null, RenderSettings.GetDefaultRenderSettings());
            Assert.Equal("Bar", output);
        }
    }
}
