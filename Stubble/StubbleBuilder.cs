using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;
using Stubble.Core.Interfaces;

namespace Stubble.Core
{
    public sealed class StubbleBuilder : IStubbleBuilder
    {
        private readonly IDictionary<Type, Func<object, string, object>> _valueGetters =
            new Dictionary<Type, Func<object, string, object>>();

        public Stubble Build()
        {
            var registry = new Registry(_valueGetters);
            return new Stubble(registry);
        }

        public void AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter)
        {
            _valueGetters.Add(valueGetter);
        }
        
        public void AddValueGetter(Type type, Func<object, string, object> valueGetter)
        {
            AddValueGetter(new KeyValuePair<Type, Func<object, string, object>>(type, valueGetter));
        }
    }
}
