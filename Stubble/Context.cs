using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Helpers;

namespace Stubble.Core
{
    public class Context
    {
        private IDictionary<string, object> Cache { get; set; }
        private readonly object _view;
        private IDictionary<Type, Func<object, string, object>> ValueRegistry { get; set; }

        public Context ParentContext { get; set; }
        public dynamic View { get; set; }

        public Context(object view) : this(view, null)
        {
        }

        public Context(object view, Context parentContext)
        {
            _view = view;
            View = _view;
            Cache = new Dictionary<string, object>()
            {
                {".", _view}
            };
            ParentContext = parentContext;

            ValueRegistry = new Dictionary<Type, Func<object, string, object>>
            {
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
        }

        public object Lookup(string name)
        {
            object value = null;
            if (Cache.ContainsKey(name))
            {
                value = Cache[name];
            }
            else
            {
                var context = this;
                bool lookupHit = false;
                while (context != null)
                {
                    if (name.IndexOf('.') > 0)
                    {
                        var names = name.Split('.');
                        value = context._view;
                        for (var i = 0; i < names.Length; i++)
                        {
                            var tempValue = GetValueFromRegistry(value, names[i]);
                            if (tempValue != null)
                            {
                                if (i == names.Length - 1)
                                    lookupHit = true;

                                value = tempValue;
                            }
                            else if (i > 0)
                            {
                                return null;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else if (context._view != null)
                    {
                        value = GetValueFromRegistry(context._view, name);
                        if (value != null)
                        {
                            lookupHit = true;
                        }
                    }

                    if (lookupHit) break;

                    context = context.ParentContext;
                }

                Cache[name] = value;
            }

            //var delegateValue = value as Delegate;
            //if (delegateValue == null) return value;
            //var methodLength = delegateValue.Method.GetParameters().Length;
            //try
            //{
            //    switch (methodLength)
            //    {
            //        case 0:
            //            value = delegateValue.DynamicInvoke(null);
            //            break;
            //        case 1:
            //            value = delegateValue.DynamicInvoke(_view);
            //            break;
            //    }
            //}
            //catch
            //{
            //    value = null;
            //}

            return value;
        }

        public Context Push(object view)
        {
            return new Context(view, this);
        }

        public object GetValueFromRegistry(object value, string key)
        {
            foreach (var entry in ValueRegistry.Where(x => x.Key.IsInstanceOfType(value)))
            {
                var outputVal = entry.Value(value, key);
                if (outputVal != null) return outputVal;
            }
            return null;
        }
    }
}
