using System;
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
    }
}
