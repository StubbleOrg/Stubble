using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes;

namespace Stubble.Core
{
    public class Stubble
    {
        public Writer Writer;

        public Stubble()
        {
            Writer = new Writer();
        }

        public string Render(string template, object view)
        {
            return Render(template, view, null);
        }

        public string Render(string template, object view, IDictionary<string, string> partials)
        {
            return Writer.Render(template, view, partials);
        }
    }
}
