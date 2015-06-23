using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Tests.Fixtures
{
    public class ContextTestFixture
    {
        public Context Context { get; set; }

        public IDictionary<Type, Func<object, string, object>> Registry =
            new ReadOnlyDictionary<Type, Func<object, string, object>>
        (
            new Dictionary<Type, Func<object, string, object>>
            {
                {
                    typeof (IDictionary<string, object>),
                    (value, key) =>
                    {
                        var castValue = value as IDictionary<string, object>;
                        return castValue != null ? castValue[key] : null;
                    }
                },
                {
                    typeof (IDictionary),
                    (value, key) =>
                    {
                        var castValue = value as IDictionary;
                        return castValue != null ? castValue[key] : null;
                    }
                },
                {
                    typeof (object), (value, key) =>
                    {
                        var type = value.GetType();
                        var propertyInfo = type.GetProperty(key);
                        return propertyInfo != null ? propertyInfo.GetValue(value, null) : null;
                    }
                }
            }
        );

        public ContextTestFixture()
        {
            Context = new Context(new
            {
                Name = "parent",
                Message = "hi",
                A = new
                {
                    B = "b"
                }
            }, Registry);
        }
    }
}
