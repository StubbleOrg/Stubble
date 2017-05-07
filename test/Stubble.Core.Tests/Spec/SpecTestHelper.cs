// <copyright file="SpecTestHelper.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpYaml.Serialization;

namespace Stubble.Core.Tests.Spec
{
    public static class SpecTestHelper
    {
        public static IEnumerable<SpecTest> GetTests(string filename, bool skip)
        {
            var settings = new SerializerSettings();

            var @base = System.AppContext.BaseDirectory;
            var path = Path.Combine(@base, $"{filename}.yml");

            var yaml = new Serializer(settings);

            using (var reader = File.OpenText(path))
            {
                var data = yaml.Deserialize<SpecTestDefinition>(reader);
                return data.tests;
            }
        }
    }
}