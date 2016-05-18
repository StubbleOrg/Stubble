using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stubble.Core.Tests.Helpers;

namespace Stubble.Core.Tests.Spec
{
    public static class SpecTestHelper
    {
        public static Dictionary<string, List<string>> SkippedTests = new Dictionary<string, List<string>>
        {
            { "comments", new List<string> { "Standalone Without Newline" } },
            { "delimiters", new List<string> { "Standalone Without Newline" } },
            { "inverted", new List<string> { "Standalone Without Newline" } },
            { "partials", new List<string>
            {
                "Standalone Without Previous Line",
                "Standalone Without Newline",
                "Standalone Indentation"
            }},
            { "sections", new List<string> { "Standalone Without Newline" } },
        };

        public static IEnumerable<SpecTest> GetTests(string filename)
        {
            using (var reader = File.OpenText(string.Format("../spec/specs/{0}.json", filename)))
            {
                var data = JsonConvert.DeserializeObject<SpecTestDefinition>(reader.ReadToEnd());
                foreach (var test in data.Tests)
                {
                    test.Data = JsonHelper.ToObject((JToken)test.Data);
                }

                return data.Tests.Where(x => !SkippedTests.ContainsKey(filename) || !SkippedTests[filename].Contains(x.Name));
            }
        }
    }
}