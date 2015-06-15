using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Tests.Fixtures
{
    public class ContextTestFixture
    {
        public Context Context { get; set; }

        public ContextTestFixture()
        {
            Context = new Context(new
            {
                Name = "parent",
                Message = "hi",
                A = new
                {
                    B = "b"
                }
            });
        }
    }
}
