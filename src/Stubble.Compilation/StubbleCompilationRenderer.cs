// <copyright file="StubbleCompilationRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Stubble.Compilation.Contexts;
using Stubble.Compilation.Interfaces;
using Stubble.Compilation.Renderers;
using Stubble.Compilation.Settings;
using Stubble.Core.Exceptions;
using Stubble.Core.Loaders;

namespace Stubble.Compilation
{
    /// <summary>
    /// A renderer for compiling Mustache templates into functions
    /// </summary>
    public sealed class StubbleCompilationRenderer : IStubbleCompilationRenderer, IAsyncStubbleCompilationRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleCompilationRenderer"/> class with a default settings
        /// </summary>
        public StubbleCompilationRenderer()
            : this(new CompilerSettingsBuilder().BuildSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubbleCompilationRenderer"/> class with the provided settings
        /// </summary>
        /// <param name="settings">The override settings</param>
        public StubbleCompilationRenderer(CompilerSettings settings)
        {
            CompilerSettings = settings;
        }

        /// <summary>
        /// Gets the core Registry instance for the Renderer
        /// </summary>
        internal CompilerSettings CompilerSettings { get; }

        /// <summary>
        /// Compiles the template to a function
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <returns>The function to render the template with</returns>
        public Func<T, string> Compile<T>(string template)
        {
            return Compile<T>(template, typeof(T), null, null) as Func<T, string>;
        }

        /// <summary>
        /// Compiles the template to a function
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="settings">The settings to configure the compilation with</param>
        /// <returns>The function to render the template with</returns>
        public Func<T, string> Compile<T>(string template, CompilationSettings settings)
        {
            return Compile<T>(template, typeof(T), null, settings) as Func<T, string>;
        }

        /// <summary>
        /// Compiles the template to a function accepting a type of data
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <returns>The function to render the template with</returns>
        public Func<T, string> Compile<T>(string template, T view)
        {
            return Compile<T>(template, view.GetType(), null, null);
        }

        /// <summary>
        /// Compiles the template to a function accepting a type of data
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="settings">The settings to configure the compilation with</param>
        /// <returns>The function to render the template with</returns>
        public Func<T, string> Compile<T>(string template, T view, CompilationSettings settings)
        {
            return Compile<T>(template, view.GetType(), null, settings);
        }

        /// <summary>
        /// Compiles the template to a function accepting a type of data and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <returns>The function to render the template with</returns>
        public Func<T, string> Compile<T>(string template, T view, IDictionary<string, string> partials)
        {
            return Compile<T>(template, view.GetType(), partials, null);
        }

        /// <summary>
        /// Compiles the template to a function and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <returns>The function to render the template with</returns>
        public Func<T, string> Compile<T>(string template, IDictionary<string, string> partials)
        {
            return Compile<T>(template, typeof(T), partials, null) as Func<T, string>;
        }

        /// <summary>
        /// Compiles the template to a function and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <param name="settings">The settings to configure the compilation with</param>
        /// <returns>The function to render the template with</returns>
        public Func<T, string> Compile<T>(string template, IDictionary<string, string> partials, CompilationSettings settings)
        {
            return Compile<T>(template, typeof(T), partials, settings) as Func<T, string>;
        }

        /// <summary>
        /// Compiles the template to a function accepting a type of data and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        public Func<T, string> Compile<T>(string template, T view, IDictionary<string, string> partials, CompilationSettings settings)
        {
            return Compile<T>(template, view.GetType(), partials, settings);
        }

        /// <summary>
        /// Compiles the template to a function
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <returns>The function to render the template with</returns>
        public ValueTask<Func<T, string>> CompileAsync<T>(string template)
        {
            return CompileAsync<T>(template, typeof(T), null, null);
        }

        /// <summary>
        /// Compiles the template to a function
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        public ValueTask<Func<T, string>> CompileAsync<T>(string template, CompilationSettings settings)
        {
            return CompileAsync<T>(template, typeof(T), null, settings);
        }

        /// <summary>
        /// Compiles the template to a function and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <returns>The function to render the template with</returns>
        public ValueTask<Func<T, string>> CompileAsync<T>(string template, IDictionary<string, string> partials)
        {
            return CompileAsync<T>(template, typeof(T), partials, null);
        }

        /// <summary>
        /// Compiles the template to a function and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        public ValueTask<Func<T, string>> CompileAsync<T>(string template, IDictionary<string, string> partials, CompilationSettings settings)
        {
            return CompileAsync<T>(template, typeof(T), partials, settings);
        }

        /// <summary>
        /// Compiles the template to a function accepting a type of data
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <returns>The function to render the template with</returns>
        public ValueTask<Func<T, string>> CompileAsync<T>(string template, T view)
        {
            return CompileAsync<T>(template, view.GetType(), null, null);
        }

        /// <summary>
        /// Compiles the template to a function accepting a type of data
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        public ValueTask<Func<T, string>> CompileAsync<T>(string template, T view, CompilationSettings settings)
        {
            return CompileAsync<T>(template, view.GetType(), null, settings);
        }

        /// <summary>
        /// Compiles the template to a function accepting a type of data and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <returns>The function to render the template with</returns>
        public ValueTask<Func<T, string>> CompileAsync<T>(string template, T view, IDictionary<string, string> partials)
        {
            return CompileAsync<T>(template, view.GetType(), partials, null);
        }

        /// <summary>
        /// Compiles the template to a function accepting a type of data and some partials
        /// </summary>
        /// <typeparam name="T">The type of data to accept</typeparam>
        /// <param name="template">The template to compile</param>
        /// <param name="view">A view on the data the compiled template will accept</param>
        /// <param name="partials">The partials to use when compiling</param>
        /// <param name="settings">The settings to use for the compilation</param>
        /// <returns>The function to render the template with</returns>
        public ValueTask<Func<T, string>> CompileAsync<T>(string template, T view, IDictionary<string, string> partials, CompilationSettings settings)
        {
            return CompileAsync<T>(template, view.GetType(), partials, settings);
        }

        private Func<T, string> Compile<T>(string template, Type viewType, IDictionary<string, string> partials, CompilationSettings settings)
        {
            var loadedTemplate = CompilerSettings.TemplateLoader.Load(template);

            if (loadedTemplate == null)
            {
                throw new UnknownTemplateException("No template was found with the name '" + template + "'");
            }

            var document = CompilerSettings.Parser.Parse(loadedTemplate, CompilerSettings.DefaultTags, pipeline: CompilerSettings.ParserPipeline);

            var renderer = new CompilationRenderer<T>(CompilerSettings.RendererPipeline, CompilerSettings.MaxRecursionDepth);

            var partialsLoader = CompilerSettings.PartialTemplateLoader;
            if (partials != null && partials.Keys.Count > 0)
            {
                partialsLoader = new CompositeLoader(new DictionaryLoader(partials), CompilerSettings.PartialTemplateLoader);
            }

            var compilationContext = new CompilerContext(viewType, Expression.Parameter(viewType, "src"), CompilerSettings, partialsLoader, settings ?? CompilerSettings.CompilationSettings);

            return renderer.Compile(document, compilationContext) as Func<T, string>;
        }

        private async ValueTask<Func<T, string>> CompileAsync<T>(string template, Type viewType, IDictionary<string, string> partials, CompilationSettings settings)
        {
            var loadedTemplate = await CompilerSettings.TemplateLoader.LoadAsync(template);

            if (loadedTemplate == null)
            {
                throw new UnknownTemplateException("No template was found with the name '" + template + "'");
            }

            var document = CompilerSettings.Parser.Parse(loadedTemplate, CompilerSettings.DefaultTags, pipeline: CompilerSettings.ParserPipeline);

            var renderer = new CompilationRenderer<T>(CompilerSettings.RendererPipeline, CompilerSettings.MaxRecursionDepth);

            var partialsLoader = CompilerSettings.PartialTemplateLoader;
            if (partials != null && partials.Keys.Count > 0)
            {
                partialsLoader = new CompositeLoader(new DictionaryLoader(partials), CompilerSettings.PartialTemplateLoader);
            }

            var compilationContext = new CompilerContext(viewType, Expression.Parameter(viewType, "src"), CompilerSettings, partialsLoader, settings ?? CompilerSettings.CompilationSettings);

            return await renderer.CompileAsync(document, compilationContext) as Func<T, string>;
        }
    }
}
