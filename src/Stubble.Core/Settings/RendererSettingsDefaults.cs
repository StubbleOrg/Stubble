// <copyright file="RendererSettingsDefaults.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Stubble.Core.Classes;
using Stubble.Core.Contexts;
using Stubble.Core.Exceptions;
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

        private static readonly LimitedSizeConcurrentDictionary<Tuple<object, string>, CallSite<Func<CallSite, object, object>>>
            DynamicCallSiteCache
            = new LimitedSizeConcurrentDictionary<Tuple<object, string>, CallSite<Func<CallSite, object, object>>>(15);

        private static readonly CSharpArgumentInfo[] EmptyCSharpArgumentInfo =
        {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
        };

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
                        if (int.TryParse(key, out var intVal))
                        {
                            return value is IList castValue && intVal > -1 && intVal < castValue.Count ? castValue[intVal] : null;
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

                        if (value is not IDictionary<string, object> cast)
                        {
                            return null;
                        }

                        var caseBound = ignoreCase
                            ? new Dictionary<string, object>(cast, StringComparer.OrdinalIgnoreCase)
                            : cast;

                        return caseBound.TryGetValue(key, out var val) ? val : null;
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
                    GetValueFromDynamicByName
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
            typeof(IDynamicMetaObjectProvider),
            typeof(IDictionary),
            typeof(string)
        };

        private static object GetValueFromDynamicByName(object value, string key, bool ignoreCase)
        {
            // First try to access the value through IDictionary. This is fast and will take care of ExpandoObject.
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

            // The value implements IDynamicMetaObjectProvider but isn't an ExpandoObject. Resolve the member using
            // the CallSite API.
            if (value is IDynamicMetaObjectProvider dynamicMetaObjectProvider)
            {
                // Can't really cache MetaObjects or member names because these objects are dynamic and
                // those values could theoretically change from call to call.
                var metaObject = dynamicMetaObjectProvider
                    .GetMetaObject(Expression.Constant(dynamicMetaObjectProvider));

                var stringComparison = ignoreCase
                    ? StringComparison.InvariantCultureIgnoreCase
                    : StringComparison.InvariantCulture;

                var casedKey = metaObject
                    .GetDynamicMemberNames()
                    .FirstOrDefault(memberName => key.Equals(memberName, stringComparison));

                if (casedKey == null)
                {
                    return null;
                }

                var callSite = DynamicCallSiteCache.GetOrAdd(
                    Tuple.Create(value, casedKey),
                    _ =>
                    {
                        var binder = Binder.GetMember(
                            CSharpBinderFlags.None,
                            casedKey,
                            typeof(RendererSettingsDefaults),
                            EmptyCSharpArgumentInfo);

                        return CallSite<Func<CallSite, object, object>>.Create(binder);
                    });

                return callSite.Target(callSite, dynamicMetaObjectProvider);
            }

            return null;
        }

        private static object GetValueFromObjectByName(object value, string key, bool ignoreCase)
        {
            var objectType = value.GetType();
            if (!GettersCache.TryGetValue(objectType, out var typeLookup))
            {
                var memberLookup = ReflectionHelper.GetMemberFunctionLookup(objectType);
                CheckKeyDistinct(memberLookup.Keys, key);

                var noCase =
                    new Dictionary<string, Lazy<Func<object, object>>>(memberLookup, StringComparer.OrdinalIgnoreCase);
                typeLookup = Tuple.Create(memberLookup, noCase);

                GettersCache.AddOrUpdate(objectType, typeLookup, (_, existing) => existing);
            }

            var lookup = ignoreCase ? typeLookup.Item2 : typeLookup.Item1;

            return lookup.TryGetValue(key, out var outValue) ? outValue.Value(value) : null;
        }

        private static void CheckKeyDistinct(Dictionary<string, Lazy<Func<object, object>>>.KeyCollection keys, string key)
        {
            var count = 0;

            foreach (var item in keys)
            {
                if (item.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }

                if (count > 1)
                {
                    throw new StubbleAmbigousMatchException($"Ambiguous match found when looking up key: '{key}'");
                }
            }
        }
    }
}
