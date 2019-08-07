// <copyright file="StubbleBuilderTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Collections.Specialized;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;
using Stubble.Core.Loaders;
using Xunit;
using Stubble.Core.Settings;
using System;

namespace Stubble.Core.Tests
{
    public class StubbleBuilderTest
    {
        [Fact]
        public void It_Can_Add_Add_Value_Getters()
        {
            var builder = new StubbleBuilder()
                .Configure(b =>
                {
                    b.AddValueGetter(typeof(string), (o, s, i) => null);
                });

            var settingsBuilder = new RendererSettingsBuilder();
            builder.ConfigureSettings(settingsBuilder);

            Assert.Contains(typeof(string), settingsBuilder.ValueGetters.Keys);
            Assert.Null(settingsBuilder.ValueGetters[typeof(string)](null, null, false));
        }

        [Fact]
        public void It_Can_Add_Enumeration_Converters()
        {
            var builder = new StubbleBuilder()
                .Configure(b =>
                {
                    b.AddEnumerationConversion(typeof(NameValueCollection), (obj) => null);
                });

            var settingsBuilder = new RendererSettingsBuilder();
            builder.ConfigureSettings(settingsBuilder);

            Assert.Contains(typeof(NameValueCollection), settingsBuilder.EnumerationConverters.Keys);
            Assert.Null(settingsBuilder.EnumerationConverters[typeof(NameValueCollection)](null));
        }

        [Fact]
        public void It_Can_Add_Truthy_Checks()
        {
            var builder = new StubbleBuilder()
                .Configure(b =>
                {
                    b.AddTruthyCheck<string>((val) =>
                    {
                        return val.Equals("Foo");
                    });
                });

            var settingsBuilder = new RendererSettingsBuilder();
            builder.ConfigureSettings(settingsBuilder);

            var check = Assert.Single(settingsBuilder.TruthyChecks);
            Assert.Equal(typeof(string), check.Key);
            Assert.True(check.Value[0]("Foo"));
            Assert.False(check.Value[0]("Bar"));
        }

        [Fact]
        public void It_Can_Set_Template_Loader()
        {
            var builder = new StubbleBuilder()
                .Configure(b =>
                {
                    b.SetTemplateLoader(new DictionaryLoader(new Dictionary<string, string> { { "test", "{{foo}}" } }));
                });

            var settingsBuilder = new RendererSettingsBuilder();
            builder.ConfigureSettings(settingsBuilder);

            Assert.NotNull(settingsBuilder.TemplateLoader);
            Assert.True(settingsBuilder.TemplateLoader is DictionaryLoader);
        }

        [Fact]
        public void It_Can_Set_A_Partial_Template_Loader()
        {
            var builder = new StubbleBuilder()
                   .Configure(b =>
                   {
                       b.SetPartialTemplateLoader(new DictionaryLoader(new Dictionary<string, string> { { "test", "{{foo}}" } }));
                   });

            var settingsBuilder = new RendererSettingsBuilder();
            builder.ConfigureSettings(settingsBuilder);

            Assert.NotNull(settingsBuilder.PartialTemplateLoader);
            Assert.True(settingsBuilder.PartialTemplateLoader is DictionaryLoader);
        }

        [Fact]
        public void It_Adds_To_Composite_Loader_If_One_Is_Defined()
        {
            var builder = new StubbleBuilder()
                .Configure(b =>
                {
                    b.SetTemplateLoader(new CompositeLoader(new DictionaryLoader(new Dictionary<string, string> { { "test", "{{foo}}" } })))
                     .AddToTemplateLoader(new DictionaryLoader(new Dictionary<string, string> { { "test2", "{{bar}}" } }));
                });

            var settingsBuilder = new RendererSettingsBuilder();
            builder.ConfigureSettings(settingsBuilder);

            Assert.NotNull(settingsBuilder.TemplateLoader);
            Assert.True(settingsBuilder.TemplateLoader is CompositeLoader);
        }

        [Fact]
        public void It_Should_Be_Able_To_Set_Ignore_Case_On_Key_Lookup()
        {
            var builder = new StubbleBuilder()
               .Configure(b =>
               {
                   b.SetIgnoreCaseOnKeyLookup(true);
               });

            var settingsBuilder = new RendererSettingsBuilder();
            builder.ConfigureSettings(settingsBuilder);

            Assert.True(settingsBuilder.IgnoreCaseOnKeyLookup);
        }

        [Fact]
        public void It_Can_Build_Stubble_Instance()
        {
            var stubble = new StubbleBuilder().Build();

            Assert.NotNull(stubble);
            Assert.NotNull(stubble.RendererSettings.ValueGetters);
            Assert.True(stubble.RendererSettings.TemplateLoader is StringLoader);
            Assert.False(stubble.RendererSettings.IgnoreCaseOnKeyLookup);
            Assert.Null(stubble.RendererSettings.PartialTemplateLoader);

            Assert.NotEmpty(stubble.RendererSettings.ValueGetters);
        }

        [Fact]
        public void DifferentBuildersBuildDifferentResults()
        {
            var stubble = new CustomBuilder().Build();
            Assert.IsType<string>(stubble);
        }

        [Fact]
        public void It_Can_ConvertOneBuilderToAnother()
        {
            var stubble = new StubbleBuilder()
                            .UseCustomBuilder()
                            .Build();
            Assert.IsType<string>(stubble);
        }

        [Fact]
        public void It_Can_Override_InAn_ExtensionMethod()
        {
            var stubble = new StubbleBuilder()
                            .UseCustomBuilder()
                            .Build();
            Assert.IsType<string>(stubble);
        }

        [Fact]
        public void It_Can_Override_Encoding_Function()
        {
            string encodingFunc(string str) => str;

            var stubbleBuilder = new StubbleBuilder()
                .Configure(settings => settings.SetEncodingFunction(encodingFunc))
                .Build();

            Assert.Equal(encodingFunc, stubbleBuilder.RendererSettings.EncodingFuction);
        }
    }

    public class CustomBuilder : IStubbleBuilder<string>
    {
        public string Build()
        {
            return "";
        }
    }

    public static class CustomBuilderExtensions
    {
        public static CustomBuilder UseCustomBuilder(this StubbleBuilder builder)
        {
            return new CustomBuilder();
        }
    }
}
