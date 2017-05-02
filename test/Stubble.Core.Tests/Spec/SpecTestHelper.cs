// <copyright file="SpecTestHelper.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stubble.Core.Tests.Helpers;
using SharpYaml.Serialization;

namespace Stubble.Core.Tests.Spec
{
    public static class SpecTestHelper
    {
        public static Dictionary<string, List<string>> SkippedTests = new Dictionary<string, List<string>>
        {
            { "comments", new List<string> { "Standalone Without Newline" } },
            { "delimiters", new List<string> { "Standalone Without Newline" } },
            { "inverted", new List<string> { "Standalone Without Newline" } },
            {
                "partials", new List<string>
                {
                    "Standalone Without Previous Line",
                    "Standalone Without Newline",
                    "Standalone Indentation"
                }
            },
            { "sections", new List<string> { "Standalone Without Newline" } },
        };

        public static IEnumerable<SpecTest> GetTests(string filename, bool skip)
        {
            var settings = new SerializerSettings();

            var @base = System.AppContext.BaseDirectory;
            var path = Path.Combine(@base, $"{filename}.yml");

            var yaml = new Serializer(settings);

            using (var reader = File.OpenText(path))
            {
                var hasSkipTestsInFile = SkippedTests.ContainsKey(filename);
                var data = yaml.Deserialize<SpecTestDefinition>(reader);
                foreach (var test in data.tests)
                {
                    if(skip && hasSkipTestsInFile && SkippedTests[filename].Contains(test.name))
                    {
                        continue;
                    }

                    test.skip = hasSkipTestsInFile && SkippedTests[filename].Contains(test.name);

                    yield return test;
                }
            }
        }
    }
}