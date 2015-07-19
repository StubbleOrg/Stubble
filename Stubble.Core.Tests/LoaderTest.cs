using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Classes.Loaders;
using Stubble.Core.Interfaces;
using Xunit;

namespace Stubble.Core.Tests
{
    public class LoaderTest
    {
        [Fact]
        public void StringLoader_Should_Just_Pass_Through()
        {
            var loader = new StringLoader();
            const string template = "{{foo}}";
            var loadedTemplate = loader.Load("{{foo}}");
            Assert.Equal(template, loadedTemplate);
        }

        [Fact]
        public void CompositeLoader_Should_Be_Able_To_Contain_StringLoader()
        {
            var loader = new CompositeLoader(new IStubbleLoader[] { new StringLoader() });
            const string template = "{{foo}}";
            var loadedTemplate = loader.Load("{{foo}}");
            Assert.Equal(template, loadedTemplate);
        }

        [Fact]
        public void CompositeLoader_Should_Throw_Exception_If_No_Template_Found()
        {
            var loader = new CompositeLoader(new IStubbleLoader[] { 
                new DictionaryLoader(new Dictionary<string, string>
                {
                    { "test", "{{foo}}" }
                })
            });
            var ex = Assert.Throws<UnknownTemplateException>(() => loader.Load("test2"));
            Assert.Equal("No template was found with the name 'test2'", ex.Message);
        }

        [Fact]
        public void CompositeLoader_Should_Fall_Through()
        {
            var loader = new CompositeLoader(new IStubbleLoader[] 
            { 
                new DictionaryLoader(new Dictionary<string, string>
                {
                    { "test", "{{foo}}" }
                }),
                new StringLoader()
            });
            const string template = "{{foo}}";
            var loadedTemplate = loader.Load("{{foo}}");
            Assert.Equal(template, loadedTemplate);
        }
    }

    internal class DictionaryLoader : IStubbleLoader
    {
        private readonly IDictionary<string, string> _templates;

        public DictionaryLoader(IDictionary<string, string> templates)
        {
            _templates = templates;
        }

        public string Load(string name)
        {
            return _templates.ContainsKey(name) ? _templates[name] : null;
        }
    }
}
