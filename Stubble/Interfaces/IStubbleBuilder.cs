using System;
using System.Collections;
using System.Collections.Generic;
using Stubble.Core.Classes;

namespace Stubble.Core.Interfaces
{
    public interface IStubbleBuilder
    {
        IStubbleBuilder AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter);
        IStubbleBuilder AddValueGetter(Type type, Func<object, string, object> valueGetter);
        IStubbleBuilder AddTokenGetter(string tokenType, Func<string, Tags, ParserOutput> tokenGetter);
        IStubbleBuilder AddTokenGetter(KeyValuePair<string, Func<string, Tags, ParserOutput>> tokenGetter);
        IStubbleBuilder AddEnumerationConversion(KeyValuePair<Type, Func<object, IEnumerable>> enumerationConversion);
        IStubbleBuilder AddEnumerationConversion(Type type, Func<object, IEnumerable> enumerationConversion);
        IStubbleBuilder AddTruthyCheck(Func<object, bool?> truthyCheck);
        IStubbleBuilder SetTemplateLoader(IStubbleLoader loader);
        IStubbleBuilder SetPartialTemplateLoader(IStubbleLoader loader);
        IStubbleBuilder SetMaxRecursionDepth(int maxRecursionDepth);
        Stubble Build();
    }
}
