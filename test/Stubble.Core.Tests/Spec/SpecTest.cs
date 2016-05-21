using System.Collections.Generic;

namespace Stubble.Core.Tests.Spec
{
    public class SpecTest
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public object Data { get; set; }
        public string Template { get; set; }
        public string Expected { get; set; }
        public IDictionary<string, string> Partials { get; set; }
    }
}