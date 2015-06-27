using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Stubble.Core.Helpers;

namespace Stubble.Core.Classes
{
    public sealed class Registry
    {
        public IReadOnlyDictionary<Type, Func<object, string, object>> ValueGetters { get; private set; }

        #region Default Value Getters
        private static readonly IDictionary<Type, Func<object, string, object>> DefaultValueGetters = new Dictionary
            <Type, Func<object, string, object>>
        {
            {
                typeof (IDictionary<string, object>),
                (value, key) =>
                {
                    var castValue = value as IDictionary<string, object>;
                    return castValue != null && castValue.ContainsKey(key) ? castValue[key] : null;
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
        };
        #endregion

        public Registry()
        {
            ValueGetters = new ReadOnlyDictionary<Type, Func<object, string, object>>(DefaultValueGetters);
        }

        public Registry(IDictionary<Type, Func<object, string, object>> valueGetters)
        {
            SetValueGetters(valueGetters);
        }

        private void SetValueGetters(IDictionary<Type, Func<object, string, object>> valueGetters)
        {
            var mergedGetters = DefaultValueGetters.MergeLeft(valueGetters);

            mergedGetters = mergedGetters
                .OrderBy(x => x.Key, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable())
                .ToDictionary(item => item.Key, item => item.Value);

            ValueGetters = new ReadOnlyDictionary<Type, Func<object, string, object>>(mergedGetters);
        }
    }
}
