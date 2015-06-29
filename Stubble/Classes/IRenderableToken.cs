using System.Collections.Generic;

namespace Stubble.Core.Classes
{
    public interface IRenderableToken
    {
        string Render(Writer writer, Context context, IDictionary<string, string> partials, string originalTemplate);
    }
}
