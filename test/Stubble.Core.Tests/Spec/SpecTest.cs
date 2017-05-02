// <copyright file="SpecTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Stubble.Core.Tests.Spec
{
    public class SpecTest
    {
        public string name { get; set; }

        public string desc { get; set; }

        public object data { get; set; }

        public string template { get; set; }

        public string expected { get; set; }

        public bool skip { get; set; }

        public IDictionary<string, string> partials { get; set; }
    }
}