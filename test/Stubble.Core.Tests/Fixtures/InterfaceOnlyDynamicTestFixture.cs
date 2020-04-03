using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Stubble.Core.Tests.Fixtures
{
    public class InterfaceOnlyDynamicTestFixture : DynamicObject
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            properties[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return properties.TryGetValue(binder.Name, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return properties.Keys;
        }
    }
}
