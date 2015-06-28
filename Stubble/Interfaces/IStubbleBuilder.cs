using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Interfaces
{
    public interface IStubbleBuilder
    {
        IStubbleBuilder AddValueGetter(KeyValuePair<Type, Func<object, string, object>> valueGetter);
        IStubbleBuilder AddValueGetter(Type type, Func<object, string, object> valueGetter);
        Stubble Build();
    }
}
