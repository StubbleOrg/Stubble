// <copyright file="SpecTestDefinition.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Stubble.Core.Tests.Spec
{
    public class SpecTestDefinition
    {
        public string overview { get; set; }

        public List<SpecTest> tests { get; set; }
    }
}