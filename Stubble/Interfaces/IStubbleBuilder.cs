using System;
using System.Collections.Generic;

namespace Stubble.Core.Interfaces
{
    public interface IStubbleBuilder
    {
        IStubbleBuilder AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter);
        IStubbleBuilder AddValueGetter(Type type, Func<object, string, object> valueGetter);
        Stubble Build();
    }
}
