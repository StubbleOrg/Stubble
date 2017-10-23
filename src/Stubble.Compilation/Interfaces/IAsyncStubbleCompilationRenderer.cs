// <copyright file="IAsyncStubbleCompilationRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stubble.Compilation.Settings;

namespace Stubble.Compilation.Interfaces
{
    /// <summary>
    /// The interface for a renderer for compiling Mustache templates into functions
    /// </summary>
    public interface IAsyncStubbleCompilationRenderer
    {
        /// <summary>
        /// Compiles the template to a function
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <returns>The function to render the template with</returns>
        ValueTask<Func<T, string>> CompileAsync<T>(string template);

        /// <summary>
        /// Compiles the template to a function
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        ValueTask<Func<T, string>> CompileAsync<T>(string template, CompilationSettings settings);

        /// <summary>
        /// Compiles the template to a function and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <returns>The function to render the template with</returns>
        ValueTask<Func<T, string>> CompileAsync<T>(string template, IDictionary<string, string> partials);

        /// <summary>
        /// Compiles the template to a function and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        ValueTask<Func<T, string>> CompileAsync<T>(string template, IDictionary<string, string> partials, CompilationSettings settings);

        /// <summary>
        /// Compiles the template to a function accepting a type of data
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <returns>The function to render the template with</returns>
        ValueTask<Func<T, string>> CompileAsync<T>(string template, T view);

        /// <summary>
        /// Compiles the template to a function accepting a type of data
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        ValueTask<Func<T, string>> CompileAsync<T>(string template, T view, CompilationSettings settings);

        /// <summary>
        /// Compiles the template to a function accepting a type of data and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <returns>The function to render the template with</returns>
        ValueTask<Func<T, string>> CompileAsync<T>(string template, T view, IDictionary<string, string> partials);

        /// <summary>
        /// Compiles the template to a function accepting a type of data and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        ValueTask<Func<T, string>> CompileAsync<T>(string template, T view, IDictionary<string, string> partials, CompilationSettings settings);
    }
}
