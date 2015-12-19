// <copyright file="Registry.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Stubble.Core.Classes.Loaders;
using Stubble.Core.Classes.Tokens;
using Stubble.Core.Helpers;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes
{
    public sealed class Registry
    {
        private static readonly string[] DefaultTokenTypes = { @"\/", "=", @"\{", "!" };
        private static readonly string[] ReservedTokens = { "name", "text" }; // Name and text are used internally for tokens so must exist

        public IReadOnlyDictionary<Type, Func<object, string, object>> ValueGetters { get; private set; }

        public IReadOnlyDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters { get; private set; }

        public IReadOnlyList<Func<object, bool?>> TruthyChecks { get; private set; }

        public IReadOnlyDictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; private set; }

        public IStubbleLoader TemplateLoader { get; private set; }

        public IStubbleLoader PartialTemplateLoader { get; private set; }

        public int MaxRecursionDepth { get; private set; }

        public RenderSettings RenderSettings { get; private set; }

        internal Regex TokenMatchRegex { get; private set; }

        #region Default Value Getters
        private static readonly IDictionary<Type, Func<object, string, object>> DefaultValueGetters = new Dictionary
            <Type, Func<object, string, object>>
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
                    var castValue = value as IDictionary<string, object>;
                    return castValue != null && castValue.ContainsKey(key) ? castValue[key] : null;
                }
            },
            {
                typeof(IDictionary),
                (value, key) =>
                {
                    var castValue = value as IDictionary;
                    return castValue != null ? castValue[key] : null;
                }
            },
            {
                typeof(object), (value, key) =>
                {
                    var type = value.GetType();
                    var memberArr = type.GetMember(key, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if (memberArr.Length != 1)
                    {
                        return null;
                    }

                    var member = memberArr[0];
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            return ((FieldInfo)member).GetValue(value);
                        case MemberTypes.Property:
                            return ((PropertyInfo)member).GetValue(value, null);
                        case MemberTypes.Method:
                            var methodMember = (MethodInfo)member;
                            return methodMember.GetParameters().Length == 0
                                ? methodMember.Invoke(value, null)
                                : null;
                        default:
                            return null;
                    }
                }
            }
        };

        #endregion
        #region Default Token Getters
        private static readonly IDictionary<string, Func<string, Tags, ParserOutput>> DefaultTokenGetters = new Dictionary
            <string, Func<string, Tags, ParserOutput>>
        {
            { "#", (s, tags) => new SectionToken() { TokenType = s, Tags = tags } },
            { "^", (s, tags) => new InvertedToken() { TokenType = s } },
            { ">", (s, tags) => new PartialToken() { TokenType = s } },
            { "&", (s, tags) => new UnescapedValueToken() { TokenType = s } },
            { "name", (s, tags) => new EscapedValueToken() { TokenType = s } },
            { "text", (s, tags) => new RawValueToken() { TokenType = s } }
        };
        #endregion
        #region Default Enumeration Converters

        private static readonly IDictionary<Type, Func<object, IEnumerable>> DefaultEnumerationConverters = new Dictionary
            <Type, Func<object, IEnumerable>>();
        #endregion

        public Registry()
            : this(default(RegistrySettings))
        {
        }

        public Registry(RegistrySettings settings)
        {
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

        private void SetValueGetters(IDictionary<Type, Func<object, string, object>> valueGetters)
        {
            if (valueGetters != null)
            {
                var mergedGetters = DefaultValueGetters.MergeLeft(valueGetters);

                mergedGetters = mergedGetters
                    .OrderBy(x => x.Key, TypeBySubclassAndAssignableImpl.TypeBySubclassAndAssignable())
                    .ToDictionary(item => item.Key, item => item.Value);

                ValueGetters = new ReadOnlyDictionary<Type, Func<object, string, object>>(mergedGetters);
            }
            else
            {
                ValueGetters = new ReadOnlyDictionary<Type, Func<object, string, object>>(DefaultValueGetters);
            }
        }

        private void SetTokenGetters(IDictionary<string, Func<string, Tags, ParserOutput>> tokenGetters)
        {
            if (tokenGetters != null)
            {
                var mergedGetters = DefaultTokenGetters.MergeLeft(tokenGetters);

                TokenGetters = new ReadOnlyDictionary<string, Func<string, Tags, ParserOutput>>(mergedGetters);
            }
            else
            {
                TokenGetters = new ReadOnlyDictionary<string, Func<string, Tags, ParserOutput>>(DefaultTokenGetters);
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
                var mergedGetters = DefaultEnumerationConverters.MergeLeft(enumerationConverters);
                EnumerationConverters = new ReadOnlyDictionary<Type, Func<object, IEnumerable>>(mergedGetters);
            }
            else
            {
                EnumerationConverters = new ReadOnlyDictionary<Type, Func<object, IEnumerable>>(DefaultEnumerationConverters);
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
    }
}
