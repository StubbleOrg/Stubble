// <copyright file="StaticStubbleRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Stubble.Core.Classes;
using Stubble.Core.Settings;

namespace Stubble.Core
{
    /// <summary>
    /// Represents a Static wrapper for a standard StubbleStringRenderer instance
    /// </summary>
    public class StaticStubbleRenderer
    {
        private static readonly Lazy<StubbleVisitorRenderer> Lazy = new Lazy<StubbleVisitorRenderer>(() => new StubbleVisitorRenderer());

        /// <summary>
        /// Gets the wrapped Stubble Instance that is Lazily Instantiated
        /// </summary>
        public static StubbleVisitorRenderer Instance => Lazy.Value;

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
    }
}
