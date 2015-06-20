using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Stubble.Core.Tests
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
            }}
        };

        public static IEnumerable<SpecTest> GetTests(string filename)
        {
            using (var reader = File.OpenText(string.Format("../../../spec/specs/{0}.json", filename)))
            {
                var data = JsonConvert.DeserializeObject<SpecTestDefinition>(reader.ReadToEnd());
                foreach (var test in data.tests)
                {
                    test.data = JsonHelper.ToObject((JToken)test.data);
                }

                return data.tests.Where(x => !SkippedTests.ContainsKey(filename) || !SkippedTests[filename].Contains(x.name));
            }
        }
    }
    
    [Collection("SpecCommentTests")]
    public class CommentsTests
    {
        [Theory, MemberData("Spec_Comments")]
        public void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            var stubble = new Stubble();
            var output = stubble.Render(data.template, data.data);
            Assert.Equal(data.expected, output);
        }

        public static IEnumerable<object[]> Spec_Comments()
        {
            return SpecTestHelper.GetTests("comments").Select(test => new object[] { test });
        }
    }

    [Collection("SpecDelimiterTests")]
    public class DelimiterTests
    {
        [Theory, MemberData("Spec_Delimiters")]
        public void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            var stubble = new Stubble();
            var output = stubble.Render(data.template, data.data, data.partials);
            Assert.Equal(data.expected, output);
        }

        public static IEnumerable<object[]> Spec_Delimiters()
        {
            return SpecTestHelper.GetTests("delimiters").Select(test => new object[] { test });
        }
    }

    [Collection("SpecInterpolationTests")]
    public class InterpolationTests
    {
        private readonly ITestOutputHelper OutputStream;

        public InterpolationTests(ITestOutputHelper output)
        {
            OutputStream = output;
        }

        [Theory, MemberData("Spec_Interpolation")]
        public void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            OutputStream.WriteLine("This is output from {0}", data.name);
            var stubble = new Stubble();
            var output = stubble.Render(data.template, data.data, data.partials);
            Assert.Equal(data.expected, output);
        }

        public static IEnumerable<object[]> Spec_Interpolation()
        {
            return SpecTestHelper.GetTests("interpolation").Select(test => new object[] { test });
        }
    }

    [Collection("InvertedTestsCollection")]
    public class InvertedTests
    {
        private readonly ITestOutputHelper OutputStream;

        public InvertedTests(ITestOutputHelper output)
        {
            OutputStream = output;
        }

        [Theory, MemberData("Spec_Inverted")]
        public void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            OutputStream.WriteLine("This is output from {0}", data.name);
            var stubble = new Stubble();
            var output = stubble.Render(data.template, data.data, data.partials);
            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.expected, output);
            Assert.Equal(data.expected, output);
        }

        public static IEnumerable<object[]> Spec_Inverted()
        {
            return SpecTestHelper.GetTests("inverted").Select(test => new object[] { test });
        }
    }

    [Collection("PartialsTestsCollection")]
    public class PartialsTests
    {
        private readonly ITestOutputHelper OutputStream;

        public PartialsTests(ITestOutputHelper output)
        {
            OutputStream = output;
        }

        [Theory, MemberData("Spec_Partials")]
        public void It_Can_Pass_Spec_Tests(SpecTest data)
        {
            OutputStream.WriteLine("This is output from {0}", data.name);
            var stubble = new Stubble();
            var output = stubble.Render(data.template, data.data, data.partials);
            OutputStream.WriteLine("Expected \"{0}\", Actual \"{1}\"", data.expected, output);
            Assert.Equal(data.expected, output);
        }

        public static IEnumerable<object[]> Spec_Partials()
        {
            return SpecTestHelper.GetTests("partials").Select(test => new object[] { test });
        }
    }

    public class SpecTestDefinition
    {
        public string overview { get; set; }
        public List<SpecTest> tests { get; set; } 
    }

    public class SpecTest
    {
        public string name { get; set; }
        public string desc { get; set; }
        public object data { get; set; }
        public string template { get; set; }
        public string expected { get; set; }
        public IDictionary<string, string> partials { get; set; }
    }

    public static class JsonHelper
    {
        public static object Deserialize(string json)
        {
            return ToObject(JToken.Parse(json));
        }

        internal static object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name,
                                              prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }
    }
}
