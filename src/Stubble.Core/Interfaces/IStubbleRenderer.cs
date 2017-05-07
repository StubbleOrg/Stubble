// <copyright file="IStubbleRenderer.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Stubble.Core.Classes;
using Stubble.Core.Settings;

namespace Stubble.Core.Interfaces
{
    /// <summary>
    /// This is the main interface for all Stubble renderers
    /// </summary>
    public interface IStubbleRenderer
    {
        /// <summary>
        /// Renders the template with the given view using the writer.
        /// </summary>
        /// <param name="template">The mustache teplate to render</param>
        /// <param name="view">The data to use for rendering</param>
        /// <returns>A mustache rendered string</returns>
        string Render(string template, object view);

        /// <summary>
        /// Renders the template with the given view using the writer
        /// and the given render settings.
        /// </summary>
        /// <param name="template">The mustache teplate to render</param>
        /// <param name="view">The data to use for rendering</param>
        /// <param name="settings">Any settings you wish to override the defaults with</param>
        /// <returns>A mustache rendered string</returns>
        string Render(string template, object view, RenderSettings settings);

        /// <summary>
        /// Renders the template with the given view and partials using
        /// the writer.
        /// </summary>
        /// <param name="template">The mustache teplate to render</param>
        /// <param name="view">The data to use for rendering</param>
        /// <param name="partials">A hash of Partials</param>
        /// <returns>A mustache rendered string</returns>
        string Render(string template, object view, IDictionary<string, string> partials);

        /// <summary>
        /// Renders the template with the given view and partials using
        /// the writer and the given Render Settings
        /// </summary>
        /// <param name="template">The mustache teplate to render</param>
        /// <param name="view">The data to use for rendering</param>
        /// <param name="partials">A hash of Partials</param>
        /// <param name="settings">Any settings you wish to override the defaults with</param>
        /// <returns>A mustache rendered string</returns>
        string Render(string template, object view, IDictionary<string, string> partials, RenderSettings settings);
    }
}
