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

        public Stubble Build()
        {
            var registry = new Registry(ValueGetters);
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
    }
}
