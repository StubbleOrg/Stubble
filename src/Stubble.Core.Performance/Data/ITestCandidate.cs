using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Performance.Data
{
    public interface ITestCandidate
    {
        TimeSpan RunTest(int iterations);
    }
}
