// <copyright file="StaticStubbleRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Classes;

namespace Stubble.Core
{
    /// <summary>
    /// Represents a Static wrapper for a standard StubbleRenderer instance
    /// </summary>
    public class StaticStubbleRenderer
    {
        private static readonly Lazy<StubbleRenderer> Lazy = new Lazy<StubbleRenderer>(() => new StubbleRenderer());

        /// <summary>
        /// Gets the wrapped Stubble Instance that is Lazily Instantiated
        /// </summary>
        public static StubbleRenderer Instance => Lazy.Value;

        /// <summary>
        /// Renders the template with the given view using the writer.
        /// </summary>
        /// <param name="template">The mustache teplate to render</param>
        /// <param name="view">The data to use for rendering</param>
        /// <returns>A mustache rendered string</returns>
        public static string Render(string template, object view)
        {
            return Instance.Render(template, view);
        }

        /// <summary>
        /// Renders the template with the given view using the writer
        /// and the given render settings.
        /// </summary>
        /// <param name="template">The mustache teplate to render</param>
        /// <param name="view">The data to use for rendering</param>
        /// <param name="settings">Any settings you wish to override the defaults with</param>
        /// <returns>A mustache rendered string</returns>
        public static string Render(string template, object view, RenderSettings settings)
        {
            return Instance.Render(template, view, settings);
        }

        /// <summary>
        /// Renders the template with the given view and partials using
        /// the writer.
        /// </summary>
        /// <param name="template">The mustache teplate to render</param>
        /// <param name="view">The data to use for rendering</param>
        /// <param name="partials">A hash of Partials</param>
        /// <returns>A mustache rendered string</returns>
        public static string Render(string template, object view, IDictionary<string, string> partials)
        {
            return Instance.Render(template, view, partials);
        }

        /// <summary>
        /// Renders the template with the given view and partials using
        /// the writer and the given Render Settings
        /// </summary>
        /// <param name="template">The mustache teplate to render</param>
        /// <param name="view">The data to use for rendering</param>
        /// <param name="partials">A hash of Partials</param>
        /// <param name="settings">Any settings you wish to override the defaults with</param>
        /// <returns>A mustache rendered string</returns>
        public static string Render(string template, object view, IDictionary<string, string> partials, RenderSettings settings)
        {
            return Instance.Render(template, view, partials, settings);
        }

        /// <summary>
        /// Parses and caches the given template in the writer and returns the list
        /// of tokens it contains. Doing this ahead of time avoids the need to parse
        /// templates on the fly as they are rendered.
        ///
        /// If you don't need the result <see cref="CacheTemplate(string)"/>
        /// </summary>
        /// <param name="template">The mustache teplate to parse</param>
        /// <returns>Returns a list of tokens</returns>
        public static IList<ParserOutput> Parse(string template)
        {
            return (List<ParserOutput>)Instance.Parse(template);
        }

        /// <summary>
        /// Parses and caches the given template in the writer and returns the list
        /// of tokens it contains. Doing this ahead of time avoids the need to parse
        /// templates on the fly as they are rendered.
        ///
        /// If you don't need the result <see cref="CacheTemplate(string,Tags)"/>
        /// </summary>
        /// <param name="template">The mustache teplate to parse</param>
        /// <param name="tags">The set of tags to use for parsing</param>
        /// <returns>Returns a list of tokens</returns>
        public static IList<ParserOutput> Parse(string template, Tags tags)
        {
            return (List<ParserOutput>)Instance.Parse(template, tags);
        }

        /// <summary>
        /// Parses and caches the given template in the writer and returns the list
        /// of tokens it contains. Doing this ahead of time avoids the need to parse
        /// templates on the fly as they are rendered.
        ///
        /// If you don't need the result <see cref="CacheTemplate(string,string)"/>
        /// </summary>
        /// <param name="template">The mustache teplate to parse</param>
        /// <param name="tags">A tag string split by a space e.g. {{ }}</param>
        /// <returns>Returns a list of tokens</returns>
        public static IList<ParserOutput> Parse(string template, string tags)
        {
            return (List<ParserOutput>)Instance.Parse(template, tags);
        }

        /// <summary>
        /// Parses a template and adds the result to the writer cache.
        /// </summary>
        /// <param name="template">The mustache teplate to parse</param>
        public static void CacheTemplate(string template)
        {
            Instance.CacheTemplate(template);
        }

        /// <summary>
        /// Parses a template with given tags and adds the result to the writer cache.
        /// </summary>
        /// <param name="template">The mustache teplate to parse</param>
        /// <param name="tags">The set of tags to use for parsing</param>
        public static void CacheTemplate(string template, Tags tags)
        {
            Instance.CacheTemplate(template, tags);
        }

        /// <summary>
        /// Parses a template with given tags and adds the result to the writer cache.
        /// </summary>
        /// <param name="template">The mustache teplate to parse</param>
        /// <param name="tags">A tag string split by a space e.g. {{ }}</param>
        public static void CacheTemplate(string template, string tags)
        {
            Instance.CacheTemplate(template, tags);
        }

        /// <summary>
        /// Helper method for clearing the Template Cache on the enclosed Instance.
        /// </summary>
        public static void ClearCache()
        {
            Instance.ClearCache();
        }
    }
}
