// <copyright file="CompilerContext.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Stubble.Compilation.Helpers;
using Stubble.Compilation.Settings;
using Stubble.Core.Contexts;
using Stubble.Core.Exceptions;
using Stubble.Core.Interfaces;
using static Stubble.Compilation.Helpers.ExpressionConstants;

namespace Stubble.Compilation.Contexts
{
    /// <summary>
    /// The context for a compilation renderer
    /// </summary>
    public class CompilerContext : BaseContext<CompilerContext>
    {
        /// <summary>
        /// Gets the value cache to avoid multiple lookups
        /// </summary>
        private readonly Dictionary<string, Expression> cache = new Dictionary<string, Expression>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilerContext"/> class.
        /// </summary>
        /// <param name="view">The type for the context</param>
        /// <param name="sourceData">The source expression for the type</param>
        /// <param name="compilerSettings">The compilation settings</param>
        /// <param name="partialLoader">A loader for partial templates</param>
        /// <param name="settings">The compilation settings for the compiler</param>
        public CompilerContext(Type view, Expression sourceData, CompilerSettings compilerSettings, IStubbleLoader partialLoader, CompilationSettings settings)
            : this(view, sourceData, compilerSettings, partialLoader, settings, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilerContext"/> class with a parent context
        /// </summary>
        /// <param name="view">The type for the context</param>
        /// <param name="sourceData">The source expression for the type</param>
        /// <param name="compilerSettings">The compilation settings</param>
        /// <param name="partialLoader">A loader for partial templates</param>
        /// <param name="settings">The compilation settings for the compiler</param>
        /// <param name="parentContext">The parent context for the new context</param>
        public CompilerContext(Type view, Expression sourceData, CompilerSettings compilerSettings, IStubbleLoader partialLoader, CompilationSettings settings, CompilerContext parentContext)
            : base(partialLoader, parentContext)
        {
            CompilerSettings = compilerSettings;
            CompilationSettings = settings;
            View = view;
            SourceData = sourceData;
            cache = new Dictionary<string, Expression>()
            {
                { ".", sourceData }
            };
        }

        /// <summary>
        /// Gets or sets the source data for the current context
        /// </summary>
        public Expression SourceData { get; protected set; }

        /// <summary>
        /// Gets the current contexts given type
        /// </summary>
        public Type View { get; }

        /// <summary>
        /// Gets the Compilation Settings
        /// </summary>
        public CompilerSettings CompilerSettings { get; }

        /// <summary>
        /// Gets the settings that affect the compilation
        /// </summary>
        public CompilationSettings CompilationSettings { get; }

        /// <summary>
        /// Pushes object into a new context with the current context as the parent context
        /// </summary>
        /// <param name="newView">The object to be the data for the context</param>
        /// <returns>A child context with the new view</returns>
        public override CompilerContext Push(object newView)
        {
            return new CompilerContext(newView as Type, SourceData, CompilerSettings, PartialLoader, CompilationSettings, this);
        }

        /// <summary>
        /// Pushes the type into a new context with the current context as the parent context with the given expression as the source
        /// </summary>
        /// <param name="newView">The object to be the data for the context</param>
        /// <param name="sourceData">The object to use as source for retrieving data</param>
        /// <returns>A child context with the new view and source</returns>
        public CompilerContext Push(object newView, Expression sourceData)
        {
            return new CompilerContext(newView as Type, sourceData, CompilerSettings, PartialLoader, CompilationSettings, this);
        }

        /// <summary>
        /// Looks up a value by name from the context
        /// </summary>
        /// <param name="name">The name of the value to lookup</param>
        /// <exception cref="StubbleDataMissException">If ThrowOnDataMiss set then thrown on value not found</exception>
        /// <returns>The value if found or null if not</returns>
        public Expression Lookup(string name)
        {
            var instance = SourceData;
            Expression value = null;

            if (cache.TryGetValue(name, out var outValue))
            {
                value = outValue;
            }
            else
            {
                var context = this;
                var lookupHit = false;
                while (context != null)
                {
                    var type = context.View;

                    if (name.IndexOf('.') > 0)
                    {
                        var names = name.Split('.');

                        for (var i = 0; i < names.Length; i++)
                        {
                            var tempValue = GetValueFromRegistry(type, instance, names[i]);
                            if (tempValue.Expression != null && tempValue.Type != null)
                            {
                                if (i == names.Length - 1)
                                {
                                    lookupHit = true;
                                }

                                type = tempValue.Type;
                                instance = tempValue.Expression;
                                value = tempValue.Expression;
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
                    else if (type != null)
                    {
                        var tempValue = GetValueFromRegistry(type, instance, name);
                        if (tempValue.Expression != null)
                        {
                            lookupHit = true;
                            value = tempValue.Expression;
                        }
                    }

                    if (lookupHit || CompilationSettings.SkipRecursiveLookup)
                    {
                        break;
                    }

                    context = context.ParentContext;
                    instance = context?.SourceData;
                }

                value = TryEnumerationConversionIfRequired(value);

                cache[name] = value;
            }

            if (!CompilationSettings.ThrowOnDataMiss || value != null)
            {
                return value;
            }

            var ex = new StubbleDataMissException($"'{name}' is undefined.");
            ex.Data["Name"] = name;
            ex.Data["SkipRecursiveLookup"] = CompilationSettings.SkipRecursiveLookup;
            throw ex;
        }

        /// <summary>
        /// Returns an Expression checking if the provided Expression is Truthy
        /// </summary>
        /// <param name="value">The value to check is truthyness</param>
        /// <returns>An expression checking for truthyness</returns>
        public Expression GetTruthyExpression(Expression value)
        {
            var parameter = Expression.Parameter(value.Type, "checkThis");
            var checks = new List<Tuple<Expression, ConstantExpression>>();

            if (!parameter.Type.GetIsValueType())
            {
                checks.Add(Tuple.Create<Expression, ConstantExpression>(Expression.Equal(parameter, Expression.Constant(null)), FalseConstant));
            }

            if (CompilerSettings.TruthyChecks.TryGetValue(parameter.Type, out var typeTruthyChecks))
            {
                checks.AddRange(typeTruthyChecks.Select(c => Tuple.Create<Expression, ConstantExpression>(Expression.Equal(Expression.Invoke(c, parameter), FalseConstant), FalseConstant)));
            }

            if (parameter.Type == typeof(bool))
            {
                checks.Add(Tuple.Create<Expression, ConstantExpression>(Expression.Equal(parameter, FalseConstant), FalseConstant));
            }
            else if (TypeHelper.NumericTypes.Contains(parameter.Type))
            {
                var zeroInType = Convert.ChangeType(0, parameter.Type);

                checks.Add(Tuple.Create<Expression, ConstantExpression>(Expression.Equal(parameter, Expression.Constant(zeroInType)), FalseConstant));
            }
            else if (parameter.Type == typeof(string))
            {
                checks.AddRange(new[]
                {
                    Tuple.Create<Expression, ConstantExpression>(Expression.Equal(parameter, Expression.Constant("1")), TrueConstant),
                    Tuple.Create<Expression, ConstantExpression>(Expression.Call(parameter, MethodInfos.Instance.StringEqualWithComparison, Expression.Constant("true"), Expression.Constant(StringComparison.OrdinalIgnoreCase)), TrueConstant),
                    Tuple.Create<Expression, ConstantExpression>(Expression.Equal(parameter, Expression.Constant("0")), FalseConstant),
                    Tuple.Create<Expression, ConstantExpression>(Expression.Call(parameter, MethodInfos.Instance.StringEqualWithComparison, Expression.Constant("false"), Expression.Constant(StringComparison.OrdinalIgnoreCase)), FalseConstant),
                    Tuple.Create<Expression, ConstantExpression>(Expression.Call(null, MethodInfos.Instance.StringIsNullOrWhitespace, parameter), FalseConstant)
                });
            }
            else if (typeof(IEnumerable).IsAssignableFrom(parameter.Type))
            {
                checks.Add(Tuple.Create<Expression, ConstantExpression>(Expression.Equal(Expression.Call(Expression.Call(parameter, MethodInfos.Instance.GetEnumerator), MethodInfos.Instance.MoveNext), FalseConstant), FalseConstant));
            }

            if (checks.Count == 0)
            {
                return null;
            }

            var returnTarget = Expression.Label(typeof(bool));
            var allChecks = checks.Select(check => (Expression)Expression.IfThen(check.Item1, Expression.Return(returnTarget, check.Item2)))
                .Concat(new[] { Expression.Label(returnTarget, Expression.Constant(true)) });

            var lambda = Expression.Lambda(Expression.Block(allChecks), parameter);

            return Expression.Invoke(lambda, value);
        }

        /// <summary>
        /// Returns all the source data Expressions from the stack
        /// </summary>
        /// <returns>The source data expressions</returns>
        public IEnumerable<Expression> GetNestedSourceData()
        {
            yield return SourceData;
            var parent = ParentContext;
            while (parent != null)
            {
                yield return parent.SourceData;
                parent = parent.ParentContext;
            }
        }

        /// <summary>
        /// Gets a value from the registry using the initalized value getters
        /// </summary>
        /// <param name="value">The value to lookup the value within</param>
        /// <param name="instance">The instance to look the value up from</param>
        /// <param name="key">The key to lookup in the value</param>
        /// <returns>The value if found or null if not</returns>
        protected RegistryResult GetValueFromRegistry(Type value, Expression instance, string key)
        {
            foreach (var entry in CompilerSettings.ValueGetters)
            {
                if (!entry.Key.IsAssignableFrom(value))
                {
                    continue;
                }

                var outputVal = entry.Value(value, instance, key, CompilerSettings.IgnoreCaseOnKeyLookup);
                if (outputVal != null)
                {
                    return new RegistryResult(outputVal.Type, outputVal);
                }
            }

            return default(RegistryResult);
        }

        /// <summary>
        /// Tries to convert an object into an Enumeration if possible
        /// </summary>
        /// <param name="value">The object to try convert</param>
        /// <returns>The passed value or the value after conversion</returns>
        private Expression TryEnumerationConversionIfRequired(Expression value)
        {
            if (value != null && CompilerSettings.EnumerationConverters.TryGetValue(value.Type, out var converter))
            {
                var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(converter.ChildType);
                return Expression.Convert(
                    Expression.Invoke(converter.ConverterExpression, value),
                    genericEnumerable);
            }

            return value;
        }
    }
}
