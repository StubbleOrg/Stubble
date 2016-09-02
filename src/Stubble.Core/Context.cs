// <copyright file="Context.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Exceptions;

namespace Stubble.Core
{
    /// <summary>
    /// Represents the context for a template
    /// </summary>
    public sealed class Context
    {
        private readonly object view;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="view">The data view to create the context with</param>
        /// <param name="registry">A reference to the a registry instance</param>
        /// <param name="settings">The render settings </param>
        public Context(object view, Registry registry, RenderSettings settings)
            : this(view, registry, null, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="view">The data view to create the context with</param>
        /// <param name="registry">A reference to the a registry instance</param>
        /// <param name="parentContext">The parent context for the new context</param>
        /// <param name="settings">The render settings </param>
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

        /// <summary>
        /// Gets the parent context of the current context
        /// </summary>
        public Context ParentContext { get; }

        /// <summary>
        /// Gets the data view of the context
        /// </summary>
        public dynamic View { get; }

        /// <summary>
        /// Gets the render settings for the context
        /// </summary>
        internal RenderSettings RenderSettings { get; }

        /// <summary>
        /// Gets the registry for the context
        /// </summary>
        internal Registry Registry { get; }

        /// <summary>
        /// Gets the value cache to avoid multiple lookups
        /// </summary>
        private IDictionary<string, object> Cache { get; }

        /// <summary>
        /// Looks up a value by name from the context
        /// </summary>
        /// <param name="name">The name of the value to lookup</param>
        /// <exception cref="StubbleDataMissException">If ThrowOnDataMiss set then thrown on value not found</exception>
        /// <returns>The value if found or null if not</returns>
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

            var ex = new StubbleDataMissException($"'{name}' is undefined.");
            ex.Data["Name"] = name;
            ex.Data["SkipRecursiveLookup"] = RenderSettings.SkipRecursiveLookup;
            throw ex;
        }

        /// <summary>
        /// Checks if the passed value is Truthy
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>If the value is truthy</returns>
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

            if (value is bool)
            {
                return (bool)value;
            }

            var strValue = value as string;
            if (strValue != null)
            {
                var trimmed = strValue.Trim();

                if (trimmed == "1")
                {
                    return true;
                }

                if (trimmed == "0")
                {
                    return false;
                }

                if (trimmed.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (trimmed.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return !string.IsNullOrEmpty(trimmed);
            }

            var enumerableValue = value as IEnumerable;
            if (enumerableValue != null)
            {
                return enumerableValue.GetEnumerator().MoveNext();
            }

            return true;
        }

        /// <summary>
        /// Returns a new <see cref="Context"/> with the given view and it's
        /// parent set as the current context
        /// </summary>
        /// <param name="newView">The data view to create the new context with</param>
        /// <returns>A new child data context of the current context</returns>
        public Context Push(object newView)
        {
            return new Context(newView, Registry, this, RenderSettings);
        }

        /// <summary>
        /// Gets a value from the registry using the initalized value getters
        /// </summary>
        /// <param name="value">The value to lookup the value within</param>
        /// <param name="key">The key to lookup in the value</param>
        /// <returns>The value if found or null if not</returns>
        private object GetValueFromRegistry(object value, string key)
        {
            foreach (var entry in Registry.ValueGetters)
            {
                if (!entry.Key.IsInstanceOfType(value))
                {
                    continue;
                }

                var outputVal = entry.Value(value, key);
                if (outputVal != null)
                {
                    return outputVal;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to convert an object into an Enumeration if possible
        /// </summary>
        /// <param name="value">The object to try convert</param>
        /// <returns>The passed value or the value after conversion</returns>
        private object TryEnumerationConversionIfRequired(object value)
        {
            if (value != null && Registry.EnumerationConverters.ContainsKey(value.GetType()))
            {
                return Registry.EnumerationConverters[value.GetType()].Invoke(value);
            }

            return value;
        }
    }
}
