// <copyright file="RendererSettingsDefaults.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        /// Returns the default value getters
        /// </summary>
        /// <param name="ignoreCase">If case should be ignored on lookup</param>
        /// <returns>The default value getters</returns>
        public static Dictionary<Type, Func<object, string, object>> DefaultValueGetters(
            bool ignoreCase)
        {
            return new Dictionary<Type, Func<object, string, object>>()
            {
                {
                    typeof(IList),
                    (value, key) =>
                    {
                        var castValue = value as IList;

                        if (int.TryParse(key, out int intVal))
                        {
                            return castValue != null && intVal < castValue.Count ? castValue[intVal] : null;
                        }

                        return null;
                    }
                },
                {
                    typeof(IDictionary<string, object>),
                    (value, key) =>
                    {
                        var castValue = ignoreCase
                            ? new Dictionary<string, object>(
                                (IDictionary<string, object>)value,
                                StringComparer.OrdinalIgnoreCase)
                            : value as IDictionary<string, object>;

                        return castValue != null && castValue.TryGetValue(key, out object outValue) ? outValue : null;
                    }
                },
                {
                    typeof(IDictionary),
                    (value, key) =>
                    {
                        var castValue = ignoreCase
                            ? new Dictionary<string, object>(
                                (IDictionary<string, object>)value,
                                StringComparer.OrdinalIgnoreCase)
                            : value as IDictionary;

                        return castValue?[key];
                    }
                },
                {
                    typeof(object), (value, key) => GetValueFromObjectByName(value, key, ignoreCase)
                }
            };
        }

        /// <summary>
        /// Returns the default token renderers
        /// </summary>
        /// <returns>A list of the default token renderers</returns>
        public static List<ITokenRenderer> DefaultTokenRenderers()
        {
            return new List<ITokenRenderer>
            {
                new SectionTokenRenderer(),
                new LiteralTokenRenderer(),
                new InterpolationTokenRenderer(),
                new PartialTokenRenderer(),
                new InvertedSectionTokenRenderer(),
            };
        }

        private static object GetValueFromObjectByName(object value, string key, bool ignoreCase)
        {
            var objectType = value.GetType();
            Tuple<Dictionary<string, Lazy<Func<object, object>>>, Dictionary<string, Lazy<Func<object, object>>>> typeLookup;
            if (!GettersCache.TryGetValue(objectType, out typeLookup))
            {
                var memberLookup = ReflectionHelper.GetMemberFunctionLookup(objectType);
                var noCase =
                    new Dictionary<string, Lazy<Func<object, object>>>(memberLookup, StringComparer.OrdinalIgnoreCase);
                typeLookup = Tuple.Create(memberLookup, noCase);

                GettersCache.AddOrUpdate(objectType, typeLookup, (_, existing) => existing);
            }

            var lookup = ignoreCase ? typeLookup.Item2 : typeLookup.Item1;

            return lookup.TryGetValue(key, out Lazy<Func<object, object>> outValue) ? outValue.Value(value) : null;
        }
    }
}
