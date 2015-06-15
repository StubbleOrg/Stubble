using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Tests.Fixtures
{
    public class ChildContextTestFixture : ContextTestFixture
    {
        public ChildContextTestFixture()
        {
            Context = Context.Push(new
            {
                Name = "child",
                C = new
                {
                    D = "d"
                }
            });
        }
    }
}
