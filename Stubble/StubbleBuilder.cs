// <copyright file="StubbleBuilder.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Loaders;
using Stubble.Core.Interfaces;

namespace Stubble.Core
{
    public sealed class StubbleBuilder : IStubbleBuilder
    {
        internal readonly IDictionary<Type, Func<object, string, object>> ValueGetters =
            new Dictionary<Type, Func<object, string, object>>();
        internal readonly IDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters =
            new Dictionary<string, Func<string, Tags, ParserOutput>>();
        internal readonly List<Func<object, bool?>> TruthyChecks =
            new List<Func<object, bool?>>();
        internal readonly IDictionary<Type, Func<object, IEnumerable>> EnumerationConverters =
            new Dictionary<Type, Func<object, IEnumerable>>();
        internal IStubbleLoader TemplateLoader = new StringLoader();
        internal IStubbleLoader PartialTemplateLoader;
        internal int MaxRecursionDepth = 256;

        public StubbleRenderer Build()
        {
            var registry = new Registry(new RegistrySettings
            {
                ValueGetters = ValueGetters,
                TokenGetters = TokenGetters,
                TruthyChecks = TruthyChecks,
                TemplateLoader = TemplateLoader,
                PartialTemplateLoader = PartialTemplateLoader,
                MaxRecursionDepth = MaxRecursionDepth,
                EnumerationConverters = EnumerationConverters
            });
            return new StubbleRenderer(registry);
        }

        public IStubbleBuilder AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter)
        {
            ValueGetters.Add(valueGetter);
            return this;
        }

        public IStubbleBuilder AddValueGetter(Type type, Func<object, string, object> valueGetter)
        {
            return AddValueGetter(new KeyValuePair<Type, Func<object, string, object>>(type, valueGetter));
        }

        public IStubbleBuilder AddTokenGetter(string tokenType, Func<string, Tags, ParserOutput> tokenGetter)
        {
            return AddTokenGetter(new KeyValuePair<string, Func<string, Tags, ParserOutput>>(tokenType, tokenGetter));
        }

        public IStubbleBuilder AddTokenGetter(KeyValuePair<string, Func<string, Tags, ParserOutput>> tokenGetter)
        {
            TokenGetters.Add(tokenGetter);
            return this;
        }

        public IStubbleBuilder AddEnumerationConversion(KeyValuePair<Type, Func<object, IEnumerable>> enumerationConversion)
        {
            EnumerationConverters.Add(enumerationConversion);
            return this;
        }

        public IStubbleBuilder AddEnumerationConversion(Type type, Func<object, IEnumerable> enumerationConversion)
        {
            return AddEnumerationConversion(new KeyValuePair<Type, Func<object, IEnumerable>>(type, enumerationConversion));
        }

        public IStubbleBuilder AddTruthyCheck(Func<object, bool?> truthyCheck)
        {
            TruthyChecks.Add(truthyCheck);
            return this;
        }

        public IStubbleBuilder AddToTemplateLoader(IStubbleLoader loader)
        {
            if (TemplateLoader is CompositeLoader)
            {
                var composite = (CompositeLoader)TemplateLoader;
                composite.AddLoader(loader);
            }
            else
            {
                var composite = new CompositeLoader(TemplateLoader, loader);
                TemplateLoader = composite;
            }
            return this;
        }

        public IStubbleBuilder SetTemplateLoader(IStubbleLoader loader)
        {
            TemplateLoader = loader;
            return this;
        }

        public IStubbleBuilder SetPartialTemplateLoader(IStubbleLoader loader)
        {
            PartialTemplateLoader = loader;
            return this;
        }

        public IStubbleBuilder AddToPartialTemplateLoader(IStubbleLoader loader)
        {
            if (PartialTemplateLoader is CompositeLoader)
            {
                var composite = (CompositeLoader)TemplateLoader;
                composite.AddLoader(loader);
            }
            else
            {
                var composite = new CompositeLoader(TemplateLoader, loader);
                PartialTemplateLoader = composite;
            }
            return this;
        }

        public IStubbleBuilder SetMaxRecursionDepth(int maxRecursionDepth)
        {
            MaxRecursionDepth = maxRecursionDepth;
            return this;
        }
    }
}
