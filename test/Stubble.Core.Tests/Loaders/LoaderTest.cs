using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Classes.Loaders;
using Xunit;

namespace Stubble.Core.Tests.Loaders
{
    public class LoaderTest
    {
        [Fact]
        public void StringLoader_Should_Be_Distinct()
        {
            var loader = new StringLoader();
            var clone = loader.Clone();

            Assert.IsType<StringLoader>(clone);
            Assert.NotEqual(clone, loader);
        }

        [Fact]
        public void CompositeLoader_ThrowsOnNull()
        {
            var composite = new CompositeLoader();
            Assert.Throws<ArgumentNullException>(() => composite.AddLoader(null));
        }

        [Fact]
        public void CompositeLoader_DoesntAddNulls()
        {
            var composite = new CompositeLoader();
            composite.AddLoaders(new StringLoader(), null);
            Assert.Equal(1, composite.Loaders.Count);
        }
    }
}
