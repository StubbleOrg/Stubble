// <copyright file="Registry.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Stubble.Core.Classes.Loaders;
using Stubble.Core.Classes.Tokens;
using Stubble.Core.Helpers;
using Stubble.Core.Interfaces;
using System.Collections.Concurrent;

namespace Stubble.Core.Classes
{
    /// <summary>
    /// A class holding the instance data for a Stubble Renderer
    /// </summary>
    public sealed class Registry
    {
        private static readonly string[] DefaultTokenTypes = { @"\/", "=", @"\{", "!" };
        private static readonly string[] ReservedTokens = { "name", "text" }; // Name and text are used internally for tokens so must exist

        /// <summary>
        /// Initializes a new instance of the <see cref="Registry"/> class
        /// with default <see cref="RegistrySettings"/>
        /// </summary>
        public Registry()
            : this(default(RegistrySettings))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Registry"/> class
        /// with given <see cref="RegistrySettings"/>
        /// </summary>
        /// <param name="settings">The registry settings to initalise the Registry with</param>
        public Registry(RegistrySettings settings)
        {
            IgnoreCaseOnKeyLookup = settings.IgnoreCaseOnKeyLookup;
            SetValueGetters(settings.ValueGetters);
            SetTokenGetters(settings.TokenGetters);
            SetTruthyChecks(settings.TruthyChecks);
            SetEnumerationConverters(settings.EnumerationConverters);
            SetTemplateLoader(settings.TemplateLoader);
            SetPartialTemplateLoader(settings.PartialTemplateLoader);
            SetTokenMatchRegex();
            MaxRecursionDepth = settings.MaxRecursionDepth ?? 256;
            RenderSettings = settings.RenderSettings ?? RenderSettings.GetDefaultRenderSettings();
        }

        /// <summary>
        /// Gets a readonly dictionary of Value Getter functions
        /// </summary>
        public IReadOnlyDictionary<Type, Func<object, string, object>> ValueGetters { get; private set; }

        /// <summary>
        /// Gets a readonly dictionary of Token Getter functions
        /// </summary>
        public IReadOnlyDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters { get; private set; }

        /// <summary>
        /// Gets a readonly list of Truthy Checks
        /// </summary>
        public IReadOnlyList<Func<object, bool?>> TruthyChecks { get; private set; }

        /// <summary>
        /// Gets a readonly dictionary of EnumerationConverters
        /// </summary>
        public IReadOnlyDictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; private set; }

        /// <summary>
        /// Gets the template loader for the Stubble instance
        /// </summary>
        public IStubbleLoader TemplateLoader { get; private set; }

        /// <summary>
        /// Gets the partial template loader for the Stubble instance
        /// </summary>
        public IStubbleLoader PartialTemplateLoader { get; private set; }

        /// <summary>
        /// Gets the max recursion depth for the render call
        /// </summary>
        public int MaxRecursionDepth { get; private set; }

        /// <summary>
        /// Gets the <see cref="RenderSettings"/> for the Stubble instance
        /// </summary>
        public RenderSettings RenderSettings { get; private set; }

        /// <summary>
        /// Gets a value indicating whether key lookups should ignore case
        /// </summary>
        public bool IgnoreCaseOnKeyLookup { get; private set; }

        /// <summary>
        /// Gets the generated Token match regex
        /// </summary>
        internal Regex TokenMatchRegex { get; private set; }

        private void SetValueGetters(IDictionary<Type, Func<object, string, object>> valueGetters)
        {
            if (valueGetters != null)
            {
                var mergedGetters = RegistryDefaults.GetDefaultValueGetters(IgnoreCaseOnKeyLookup).MergeLeft(valueGetters);

                mergedGetters = mergedGetters
                    .OrderBy(x => x.Key, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable())
                    .ToDictionary(item => item.Key, item => item.Value);

                ValueGetters = new ReadOnlyDictionary<Type, Func<object, string, object>>(mergedGetters);
            }
            else
            {
                ValueGetters = new ReadOnlyDictionary<Type, Func<object, string, object>>(RegistryDefaults.GetDefaultValueGetters(IgnoreCaseOnKeyLookup));
            }
        }

        private void SetTokenGetters(IDictionary<string, Func<string, Tags, ParserOutput>> tokenGetters)
        {
            if (tokenGetters != null)
            {
                var mergedGetters = RegistryDefaults.DefaultTokenGetters.MergeLeft(tokenGetters);

                TokenGetters = new ReadOnlyDictionary<string, Func<string, Tags, ParserOutput>>(mergedGetters);
            }
            else
            {
                TokenGetters = new ReadOnlyDictionary<string, Func<string, Tags, ParserOutput>>(RegistryDefaults.DefaultTokenGetters);
            }
        }

        private void SetTruthyChecks(IReadOnlyList<Func<object, bool?>> truthyChecks)
        {
            TruthyChecks = truthyChecks ?? new List<Func<object, bool?>>();
        }

        private void SetEnumerationConverters(IDictionary<Type, Func<object, IEnumerable>> enumerationConverters)
        {
            if (enumerationConverters != null)
            {
                var mergedGetters = RegistryDefaults.DefaultEnumerationConverters.MergeLeft(enumerationConverters);
                EnumerationConverters = new ReadOnlyDictionary<Type, Func<object, IEnumerable>>(mergedGetters);
            }
            else
            {
                EnumerationConverters = new ReadOnlyDictionary<Type, Func<object, IEnumerable>>(RegistryDefaults.DefaultEnumerationConverters);
            }
        }

        private void SetTemplateLoader(IStubbleLoader loader)
        {
            TemplateLoader = loader ?? new StringLoader();
        }

        private void SetPartialTemplateLoader(IStubbleLoader loader)
        {
            PartialTemplateLoader = loader;
        }

        private void SetTokenMatchRegex()
        {
            TokenMatchRegex = new Regex(
                string.Join("|", TokenGetters.Where(s => !ReservedTokens.Contains(s.Key))
                                        .Select(s => Parser.EscapeRegexExpression(s.Key))
                                        .Concat(DefaultTokenTypes)));
        }

        private static class RegistryDefaults
        {
            public static readonly IDictionary<string, Func<string, Tags, ParserOutput>> DefaultTokenGetters = new Dictionary
                <string, Func<string, Tags, ParserOutput>>
            {
                { "#", (s, tags) => new SectionToken(tags) { TokenType = s } },
                { "^", (s, tags) => new InvertedToken { TokenType = s } },
                { ">", (s, tags) => new PartialToken { TokenType = s } },
                { "&", (s, tags) => new UnescapedValueToken { TokenType = s } },
                { "name", (s, tags) => new EscapedValueToken { TokenType = s } },
                { "text", (s, tags) => new RawValueToken { TokenType = s } }
            };

            public static readonly IDictionary<Type, Func<object, IEnumerable>> DefaultEnumerationConverters = new Dictionary
                <Type, Func<object, IEnumerable>>();

            public static IDictionary<Type, Func<object, string, object>> GetDefaultValueGetters(
                bool ignoreCase)
            {
                return new Dictionary<Type, Func<object, string, object>>()
                {
                    {
                        typeof(IList),
                        (value, key) =>
                        {
                            var castValue = value as IList;

                            int intVal;
                            if (int.TryParse(key, out intVal))
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

                            return castValue != null && castValue.ContainsKey(key) ? castValue[key] : null;
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
                        typeof(object), (value, key) => GetValueFromObjectByName_New(value, key, ignoreCase)
                    }
                };
            }

            private static readonly ConcurrentDictionary<Type, Tuple<Dictionary<string, Func<object, object>>, Dictionary<string, Func<object, object>>>> GettersCache
                = new ConcurrentDictionary<Type, Tuple<Dictionary<string, Func<object, object>>, Dictionary<string, Func<object, object>>>>();

            private static object GetValueFromObjectByName_New(object value, string key, bool ignoreCase)
            {
                var objectType = value.GetType();
                Tuple<Dictionary<string, Func<object, object>>, Dictionary<string, Func<object, object>>> typeLookup;
                if (!GettersCache.TryGetValue(objectType, out typeLookup))
                {
                    var memberLookup = GetMemberLookup(objectType);
                    var noCase = memberLookup.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
                    typeLookup = Tuple.Create(memberLookup, noCase);

                    GettersCache.AddOrUpdate(objectType, typeLookup, (_, existing) => existing);
                }

                var lookup = ignoreCase ? typeLookup.Item2 : typeLookup.Item1;

                return lookup.ContainsKey(key) ? lookup[key](value) : null;
            }

            private static Dictionary<string, Func<object, object>> GetMemberLookup(Type objectType)
            {
                var members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                                      BindingFlags.FlattenHierarchy);

                return members.Select(m =>
                {
                    Expression ex = null;
                    var param = Expression.Parameter(typeof(object));
                    var cast = Expression.Convert(param, objectType);

                    if (m is FieldInfo)
                    {
                        var fi = m as FieldInfo;

                        ex = fi.IsStatic ? Expression.Field(null, fi) : Expression.Field(cast, fi);
                    }
                    if (m is PropertyInfo)
                    {
                        var pi = m as PropertyInfo;
                        var mi = pi.GetGetMethod();
                        ex = mi.IsStatic ? Expression.Call(mi) : (Expression)Expression.Property(cast, m as PropertyInfo);
                    }

                    var methodInfo = m as MethodInfo;
                    if (methodInfo != null && methodInfo.GetParameters().Length == 0 && !methodInfo.IsGenericMethod && methodInfo.ReturnType != typeof(void))
                    {
                        ex = methodInfo.IsStatic ? Expression.Call(methodInfo) : Expression.Call(cast, methodInfo);
                    }

                    if (ex == null)
                    {
                        return null;
                    }

                    return new MemberInfo
                    {
                        AccessorExpression = ex,
                        Parameter = param,
                        Name = m.Name
                    };
                }).Where(mi => mi != null).ToDictionary(mi => mi.Name, mi => Expression.Lambda<Func<object, object>>(
                    Expression.Convert(mi.AccessorExpression, typeof(object)),
                    mi.Parameter
                ).Compile());
            }

            private class MemberInfo
            {
                public Expression AccessorExpression { get; set; }
                public ParameterExpression Parameter { get; set; }
                public string Name { get; set; }
            }

            private static object GetValueFromObjectByName(object value, string key, bool ignoreCase)
            {
                var type = value.GetType();

                    var bindings = ignoreCase
                        ? BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                            BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase
                        : BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                            BindingFlags.FlattenHierarchy;

                    var memberArr = type.GetMember(key, bindings);
                if (memberArr.Length != 1)
                {
                    return null;
                }

                var member = memberArr[0];
                if (member is FieldInfo)
                {
                    return ((FieldInfo)member).GetValue(value);
                }

                if (member is PropertyInfo)
                {
                    return ((PropertyInfo)member).GetValue(value, null);
                }

                if (member is MethodInfo)
                {
                    var methodMember = (MethodInfo)member;
                    return methodMember.GetParameters().Length == 0
                        ? methodMember.Invoke(value, null)
                        : null;
                }

                return null;
            }
        }
    }
}
