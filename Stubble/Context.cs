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
        private object View { get; set; }
        private Context ParentContext { get; set; }

        public Context(object view) : this(view, null)
        {
        }

        public Context(object view, Context parentContext)
        {
            View = view;
            Cache = new Dictionary<string, object>()
            {
                {".", View}
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
                        value = context.View;
                        for (var i = 0; i < names.Length; i++)
                        {
                            if (i == names.Length - 1)
                                lookupHit = value.GetType().GetProperty(names[i]) != null;

                            value = value.GetType().GetProperty(names[i]).GetValue(value, null);
                        }
                    }
                    else if (context.View != null)
                    {
                        var type = context.View.GetType();
                        var propertyInfo = type.GetProperty(name);
                        if (propertyInfo != null)
                        {
                            value = propertyInfo.GetValue(context.View, null);
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
                        value = delegateValue.DynamicInvoke(View);
                        break;
                }
            }
            catch
            {
                value = null;
            }

            return value;
        }
    }
}
