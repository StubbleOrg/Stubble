using System;
using System.Collections.Generic;
using Stubble.Core.Classes;
using Stubble.Core.Interfaces;

namespace Stubble.Core
{
    public sealed class StubbleBuilder : IStubbleBuilder
    {
        internal readonly IDictionary<Type, Func<object, string, object>> ValueGetters =
            new Dictionary<Type, Func<object, string, object>>();

        internal readonly IDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters =
            new Dictionary<string, Func<string, Tags, ParserOutput>>();

        public Stubble Build()
        {
            var registry = new Registry(ValueGetters, TokenGetters);
            return new Stubble(registry);
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
    }
}
