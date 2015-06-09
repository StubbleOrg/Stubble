using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core
{
    public class Context
    {
        private IDictionary<string, object> Cache { get; set; }
        private object View { get; set; }
        private Context ParentContext { get; set; }

        public Context(object view) : this(view, null)
        {
        }

        public Context(object view, Context parentContext)
        {
            View = view;
            Cache = new Dictionary<string, object>()
            {
                {".", View}
            };
            ParentContext = parentContext;
        }
    }
}
