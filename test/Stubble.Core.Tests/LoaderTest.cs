// <copyright file="LoaderTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Builders;
using Stubble.Core.Exceptions;
using Stubble.Core.Loaders;
using Xunit;

namespace Stubble.Core.Tests
{
    public class LoaderTest
    {
        [Fact]
        public void StringLoader_Should_Just_Pass_Through()
        {
            var loader = new StringLoader();
            const string template = "{{foo}}";
            var loadedTemplate = loader.Load("{{foo}}");
            Assert.Equal(template, loadedTemplate);
        }

        [Fact]
        public void CompositeLoader_Should_Be_Able_To_Contain_StringLoader()
        {
            var loader = new CompositeLoader(new StringLoader());
            const string template = "{{foo}}";
            var loadedTemplate = loader.Load("{{foo}}");
            Assert.Equal(template, loadedTemplate);
        }

        [Fact]
        public void CompositeLoader_Should_Throw_Exception_If_No_Template_Found()
        {
            var loader = new CompositeLoader(new DictionaryLoader(new Dictionary<string, string>
            {
                { "test", "{{foo}}" }
            }));
            var ex = Assert.Throws<UnknownTemplateException>(() => loader.Load("test2"));
            Assert.Equal("No template was found with the name 'test2'", ex.Message);
        }

        [Fact]
        public void CompositeLoader_Should_Fall_Through()
        {
            var loader = new CompositeLoader(
                new DictionaryLoader(new Dictionary<string, string>
            {
                { "test", "{{foo}}" }
            }), new StringLoader());
            const string template = "{{foo}}";
            var loadedTemplate = loader.Load("{{foo}}");
            Assert.Equal(template, loadedTemplate);
        }

        [Fact]
        public void Auto_Cascase_Loaders_On_Add()
        {
            var stubble = new StubbleBuilder()
                .AddToTemplateLoader(new DictionaryLoader(new Dictionary<string, string>
                {
                    { "Foo", "I'm Foo" },
                    { "Bar", "I'm Bar" }
                })).Build();
            Assert.Equal("I'm Foo", stubble.Render("Foo", new { foo = "bar" }));
            Assert.Equal("bar", stubble.Render("{{foo}}", new { foo = "bar" }));
        }

        [Fact]
        public void Auto_Cascase_Partial_Loaders_On_Add()
        {
            var stubble = new StubbleBuilder()
                .AddToPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string>
                {
                    { "Foo", "I'm Foo" },
                    { "Bar", "I'm Bar" }
                })).Build();
            Assert.Equal("I'm Foo", stubble.Render("{{> Foo}}", new { foo = "blah" }));
            Assert.Equal("bar", stubble.Render("{{foo}}", new { foo = "bar" }));
        }
    }
}
