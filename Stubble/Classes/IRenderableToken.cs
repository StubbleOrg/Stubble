using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes
{
    public interface IRenderableToken
    {
        string Render(Context context, IDictionary<string, string> partials, string originalTemplate);
    }
}
