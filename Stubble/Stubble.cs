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
        private Registry _registry;

        public Stubble() : this(new Registry())
        {
        }

        internal Stubble(Registry registry)
        {
            _registry = registry;
            Writer = new Writer { ValueRegistry = _registry.ValueGetters };
        }

        /// <summary>
        /// Renders the template with the given view using the writer.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="view"></param>
        /// <returns>A mustache rendered string</returns>
        public string Render(string template, object view)
        {
            return Render(template, view, null);
        }

        /// <summary>
        /// Renders the template with the given view and partials using 
        /// the writer.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="view"></param>
        /// <param name="partials">A hash of Partials</param>
        /// <returns>A mustache rendered string</returns>
        public string Render(string template, object view, IDictionary<string, string> partials)
        {
            return Writer.Render(template, view, partials);
        }

        /// <summary>
        /// Parses and caches the given template in the writer and returns the list 
        /// of tokens it contains. Doing this ahead of time avoids the need to parse 
        /// templates on the fly as they are rendered. 
        /// 
        /// If you don't need the result <see cref="CacheTemplate(string)"/>
        /// </summary>
        /// <param name="template"></param>
        /// <returns>Returns a list of tokens</returns>
        public IList<ParserOutput> Parse(string template)
        {
            return Writer.Parse(template, null);
        }

        /// <summary>
        /// Parses and caches the given template in the writer and returns the list 
        /// of tokens it contains. Doing this ahead of time avoids the need to parse 
        /// templates on the fly as they are rendered. 
        /// 
        /// If you don't need the result <see cref="CacheTemplate(string, Tags)"/>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="tags"></param>
        /// <returns>Returns a list of tokens</returns>
        public IList<ParserOutput> Parse(string template, Tags tags)
        {
            return Writer.Parse(template, tags);
        }

        /// <summary>
        /// Parses and caches the given template in the writer and returns the list 
        /// of tokens it contains. Doing this ahead of time avoids the need to parse 
        /// templates on the fly as they are rendered. 
        /// 
        /// If you don't need the result <see cref="CacheTemplate(string, string)"/>
        /// </summary>
        /// <param name="template"></param>
        /// <param name="tags"></param>
        /// <returns>Returns a list of tokens</returns>
        public IList<ParserOutput> Parse(string template, string tags)
        {
            return Parse(template, new Tags(tags.Split(' ')));
        }

        /// <summary>
        /// Parses a template and adds the result to the writer cache.
        /// </summary>
        /// <param name="template"></param>
        public void CacheTemplate(string template)
        {
            Parse(template);
        }

        /// <summary>
        /// Parses a template with given tags and adds the result to the writer cache.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="tags"></param>
        public void CacheTemplate(string template, Tags tags)
        {
            Parse(template, tags);
        }

        /// <summary>
        /// Parses a template with given tags and adds the result to the writer cache.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="tags">A tag string split by a space e.g. {{ }}</param>
        public void CacheTemplate(string template, string tags)
        {
            Parse(template, tags);
        }

        /// <summary>
        /// Clears all cached templates in the Writer.
        /// </summary>
        public void ClearCache()
        {
            Writer.ClearCache();
        }
    }
}
