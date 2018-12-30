// <copyright file="RendererSettingsDefaults.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using Stubble.Core.Contexts;
using Stubble.Core.Helpers;
using Stubble.Core.Renderers.Interfaces;
using Stubble.Core.Renderers.StringRenderer.TokenRenderers;

namespace Stubble.Core.Settings
{
    /// <summary>
    /// Contains the renderer settings defaults
    /// </summary>
    public static class RendererSettingsDefaults
    {
        private static readonly ConcurrentDictionary<Type, Tuple<Dictionary<string, Lazy<Func<object, object>>>,
            Dictionary<string, Lazy<Func<object, object>>>>> GettersCache
            =
            new ConcurrentDictionary<Type, Tuple<Dictionary<string, Lazy<Func<object, object>>>,
                Dictionary<string, Lazy<Func<object, object>>>>>();

        /// <summary>
        /// Delegate type for value getters
        /// </summary>
        /// <param name="value">The value to lookup the key within</param>
        /// <param name="key">The key to lookup</param>
        /// <param name="ignoreCase">If case should be ignored when looking up value</param>
        /// <returns>The value if found or null if not found</returns>
        public delegate object ValueGetterDelegate(object value, string key, bool ignoreCase);

        /// <summary>
        /// Returns the default value getters
        /// </summary>
        /// <returns>The default value getters</returns>
        public static Dictionary<Type, ValueGetterDelegate> DefaultValueGetters()
        {
            return new Dictionary<Type, ValueGetterDelegate>()
            {
                {
                    typeof(IList),
                    (value, key, ignoreCase) =>
                    {
                        var castValue = value as IList;

                        if (int.TryParse(key, out var intVal))
                        {
                            return castValue != null && intVal < castValue.Count ? castValue[intVal] : null;
                        }

                        return null;
                    }
                },
                {
                    typeof(IDictionary<string, object>),
                    (value, key, ignoreCase) =>
                    {
                        // Skip dynamic objects since they also implement IDictionary<string, object>
                        if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(value.GetType()))
                        {
                            return null;
                        }

                        return value is IDictionary<string, object> castValue && castValue.TryGetValue(key, out var outValue) ? outValue : null;
                    }
                },
                {
                    typeof(IDictionary),
                    (value, key, ignoreCase) =>
                    {
                        var castValue = value as IDictionary;

                        return castValue?[key];
                    }
                },
                {
                    typeof(IDynamicMetaObjectProvider),
                    (value, key, ignoreCase) =>
                    {
                        if (value is IDictionary<string, object> cast)
                        {
                            var caseBound = ignoreCase
                                ? new Dictionary<string, object>(cast, StringComparer.OrdinalIgnoreCase)
                                : cast;

                            if (caseBound.TryGetValue(key, out var val))
                            {
                                return val;
                            }
                        }

                        return null;
                    }
                },
                {
                    typeof(object),
                    GetValueFromObjectByName
                }
            };
        }

        /// <summary>
        /// Returns the default token renderers
        /// </summary>
        /// <returns>A list of the default token renderers</returns>
        public static List<ITokenRenderer<Context>> DefaultTokenRenderers()
        {
            return new List<ITokenRenderer<Context>>
            {
                new SectionTokenRenderer(),
                new LiteralTokenRenderer(),
                new InterpolationTokenRenderer(),
                new PartialTokenRenderer(),
                new InvertedSectionTokenRenderer(),
            };
        }

        /// <summary>
        /// Returns the default blacklisted types for sections
        /// </summary>
        /// <returns>A hashset of default blacklisted types for sections</returns>
        public static HashSet<Type> DefaultSectionBlacklistTypes() => new HashSet<Type>
        {
            typeof(IDictionary),
            typeof(string)
        };

        private static object GetValueFromObjectByName(object value, string key, bool ignoreCase)
        {
            var objectType = value.GetType();
            if (!GettersCache.TryGetValue(objectType, out var typeLookup))
            {
                var memberLookup = ReflectionHelper.GetMemberFunctionLookup(objectType);
                var noCase =
                    new Dictionary<string, Lazy<Func<object, object>>>(memberLookup, StringComparer.OrdinalIgnoreCase);
                typeLookup = Tuple.Create(memberLookup, noCase);

                GettersCache.AddOrUpdate(objectType, typeLookup, (_, existing) => existing);
            }

            var lookup = ignoreCase ? typeLookup.Item2 : typeLookup.Item1;

            return lookup.TryGetValue(key, out var outValue) ? outValue.Value(value) : null;
        }
    }
}
