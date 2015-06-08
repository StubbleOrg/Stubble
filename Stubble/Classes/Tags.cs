using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes
{
    public class Tags
    {
        public Tags(string startTag, string endTag)
        {
            StartTag = startTag;
            EndTag = endTag;
        }

        public Tags(string[] tags)
        {
            if (tags.Length != 2) throw new Exception("Invalid Tags");
            StartTag = tags[0];
            EndTag = tags[1];
        }

        public string StartTag { get; private set; }
        public string EndTag { get; private set; }
    }
}
