// <copyright file="RegistrySettings.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Stubble.Core.Interfaces;

namespace Stubble.Core.Classes
{
    /// <summary>
    /// Class that stores all the settings with which to initialise
    /// the <see cref="Registry"/> with
    /// </summary>
    public struct RegistrySettings
    {
        /// <summary>
        /// Gets or sets a map of Types to Value getter functions
        /// </summary>
        public IDictionary<Type, Func<object, string, object>> ValueGetters { get; set; }

        /// <summary>
        /// Gets or sets a map of Token Identifiers to Token Getter Function
        /// </summary>
        public IDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters { get; set; }

        /// <summary>
        /// Gets or sets a readonly list of TruthyChecks
        /// </summary>
        public List<Func<object, bool?>> TruthyChecks { get; set; }

        /// <summary>
        /// Gets or sets the primary Template loader
        /// </summary>
        public IStubbleLoader TemplateLoader { get; set; }

        /// <summary>
        /// Gets or sets the partial Template Loader
        /// </summary>
        public IStubbleLoader PartialTemplateLoader { get; set; }

        /// <summary>
        /// Gets or sets the MaxRecursionDepth
        /// </summary>
        public int? MaxRecursionDepth { get; set; }

        /// <summary>
        /// Gets or sets the RenderSettings
        /// </summary>
        public RenderSettings RenderSettings { get; set; }

        /// <summary>
        /// Gets or sets a map of Types to Enumeration convert functions
        /// </summary>
        public IDictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; set; }
    }
}
