using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core
{
    public class Context
    {
        private IDictionary<string, object> Cache { get; set; }
        private readonly object _view;

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
                            if (i == names.Length - 1)
                                lookupHit = value.GetType().GetProperty(names[i]) != null;

                            var propertyInfo = value.GetType().GetProperty(names[i]);
                            if (propertyInfo != null)
                            {
                                value = propertyInfo.GetValue(value, null);
                            }
                        }
                    }
                    else if (context._view != null)
                    {
                        var type = context._view.GetType();
                        var propertyInfo = type.GetProperty(name);
                        if (propertyInfo != null)
                        {
                            value = propertyInfo.GetValue(context._view, null);
                            lookupHit = true;
                        }
                    }

                    if (lookupHit) break;

                    context = context.ParentContext;
                }

                Cache[name] = value;
            }

            var delegateValue = value as Delegate;
            if (delegateValue == null) return value;
            var methodLength = delegateValue.Method.GetParameters().Length;
            try
            {
                switch (methodLength)
                {
                    case 0:
                        value = delegateValue.DynamicInvoke(null);
                        break;
                    case 1:
                        value = delegateValue.DynamicInvoke(_view);
                        break;
                }
            }
            catch
            {
                value = null;
            }

            return value;
        }

        public Context Push(object view)
        {
            return new Context(view, this);
        }
    }
}
