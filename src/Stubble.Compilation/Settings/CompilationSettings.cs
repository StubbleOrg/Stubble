// <copyright file="CompilationSettings.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Globalization;

namespace Stubble.Compilation.Settings
{
    /// <summary>
    /// The settings to be used when Rendering a Mustache Template
    /// </summary>
    public class CompilationSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether values should be recursively
        /// looked up in the render context. (faster without)
        /// </summary>
        public bool SkipRecursiveLookup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exceptions should be thrown if
        /// tags are defined which don't exist in the context
        /// </summary>
        public bool ThrowOnDataMiss { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether interpolation tokens should be html encoded by default
        /// </summary>
        public bool SkipHtmlEncoding { get; set; }

        /// <summary>
        /// Gets or sets the CultureInfo to use for rendering format-dependent values (doubles, etc.).
        /// </summary>
        public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// Gets the default render settings
        /// </summary>
        /// <returns>the default <see cref="CompilationSettings"/></returns>
        public static CompilationSettings GetDefaultRenderSettings()
        {
            return new CompilationSettings
            {
                SkipRecursiveLookup = false,
                ThrowOnDataMiss = false,
                SkipHtmlEncoding = false,
                CultureInfo = CultureInfo.InvariantCulture
            };
        }
    }
}
