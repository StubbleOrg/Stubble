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
    public struct RegistrySettings
    {
        public IDictionary<Type, Func<object, string, object>> ValueGetters { get; set; }
        public IDictionary<string, Func<string, Tags, ParserOutput>> TokenGetters { get; set; }
        public IReadOnlyList<Func<object, bool?>> TruthyChecks { get; set; }
        public IStubbleLoader TemplateLoader { get; set; }
        public IStubbleLoader PartialTemplateLoader { get; set; }
        public int? MaxRecursionDepth { get; set; }
        public RenderSettings RenderSettings { get; set; }
        public IDictionary<Type, Func<object, IEnumerable>> EnumerationConverters { get; set; }
    }
}
