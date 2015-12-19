// <copyright file="Context.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Exceptions;

namespace Stubble.Core
{
    public sealed class Context
    {
        public Context(object view, Registry registry, RenderSettings settings)
    : this(view, registry, null, settings)
        {
        }

        public Context(object view, Registry registry, Context parentContext, RenderSettings settings)
        {
            this.view = view;
            View = this.view;
            ParentContext = parentContext;
            Registry = registry;
            RenderSettings = settings;
            Cache = new Dictionary<string, object>()
            {
                { ".", TryEnumerationConversionIfRequired(this.view) }
            };
        }

        internal RenderSettings RenderSettings { get; }

        internal Registry Registry { get; }

        private IDictionary<string, object> Cache { get; set; }

        private readonly object view;

        public Context ParentContext { get; set; }

        public dynamic View { get; set; }

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
                        value = context.view;
                        for (var i = 0; i < names.Length; i++)
                        {
                            var tempValue = GetValueFromRegistry(value, names[i]);
                            if (tempValue != null)
                            {
                                if (i == names.Length - 1)
                                {
                                    lookupHit = true;
                                }

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
                    else if (context.view != null)
                    {
                        value = GetValueFromRegistry(context.view, name);
                        if (value != null)
                        {
                            lookupHit = true;
                        }
                    }

                    if (lookupHit || RenderSettings.SkipRecursiveLookup)
                    {
                        break;
                    }

                    context = context.ParentContext;
                }

                value = TryEnumerationConversionIfRequired(value);

                Cache[name] = value;
            }

            if (!RenderSettings.ThrowOnDataMiss || value != null)
            {
                return value;
            }

            var ex = new StubbleDataMissException(string.Format("'{0}' is undefined.", name));
            ex.Data["Name"] = name;
            ex.Data["SkipRecursiveLookup"] = RenderSettings.SkipRecursiveLookup;
            throw ex;
        }

        public Context Push(object view)
        {
            return new Context(view, Registry, this, RenderSettings);
        }

        public object GetValueFromRegistry(object value, string key)
        {
            foreach (var entry in Registry.ValueGetters.Where(x => x.Key.IsInstanceOfType(value)))
            {
                var outputVal = entry.Value(value, key);
                if (outputVal != null)
                {
                    return outputVal;
                }
            }

            return null;
        }

        public object TryEnumerationConversionIfRequired(object value)
        {
            if (value != null && Registry.EnumerationConverters.ContainsKey(value.GetType()))
            {
                return Registry.EnumerationConverters[value.GetType()].Invoke(value);
            }

            return value;
        }

        public bool IsTruthyValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            foreach (var func in Registry.TruthyChecks)
            {
                var funcResult = func(value);
                if (funcResult.HasValue)
                {
                    return funcResult.Value;
                }
            }

            bool boolValue;
            var parseResult = bool.TryParse(value.ToString(), out boolValue) ? (bool?)boolValue : null;
            if (parseResult.HasValue || value is bool)
            {
                return parseResult ?? (bool)value;
            }

            var strValue = value as string;
            if (strValue != null)
            {
                return !string.IsNullOrEmpty(strValue);
            }

            var enumerableValue = value as IEnumerable;
            if (enumerableValue != null)
            {
                return enumerableValue.GetEnumerator().MoveNext();
            }

            return true;
        }
    }
}
