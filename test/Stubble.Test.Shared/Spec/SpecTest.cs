// <copyright file="SpecTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Stubble.Test.Shared.Spec
{
    public class SpecTest
    {
        public string Name { get; set; }

        public string Desc { get; set; }

        public dynamic Data { get; set; }

        public string Template { get; set; }

        public string Expected { get; set; }

        public bool Skip { get; set; }

        public IDictionary<string, string> Partials { get; set; } = new Dictionary<string, string>();

        public Exception ExpectedException { get; set; }

        public CultureInfo CultureInfo { get; set; }
    }
}
