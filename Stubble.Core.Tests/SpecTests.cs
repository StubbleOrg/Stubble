using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YamlDotNet.Dynamic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Stubble.Core.Tests
{
    
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
            using (var reader = File.OpenText("../../../spec/specs/comments.yml"))
            {
                var d = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                var data = d.Deserialize<SpecTestDefinition>(reader);
                foreach (var test in data.tests)
                {
                    yield return new object[] { test };
                }
            }
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
    }
}
