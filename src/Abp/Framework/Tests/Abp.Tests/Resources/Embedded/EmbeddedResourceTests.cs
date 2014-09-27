using Abp.Resources.Embedded;
using Xunit;

namespace Abp.Tests.Resources.Embedded
{
    public class EmbeddedResourceTests
    {
        private readonly IEmbeddedResourceManager _embeddedResourceManager;

        public EmbeddedResourceTests()
        {
            _embeddedResourceManager = new EmbeddedResourceManager();
        }

        [Fact]
        public void Should_Define_And_Get_Embedded_Resources()
        {
            _embeddedResourceManager.ExposeResources("MyApp/MyResources", GetType().Assembly, "Abp.Tests.Resources.Embedded.MyResources");
            var resource = _embeddedResourceManager.GetResource("MyApp/MyResources/js/MyScriptFile1.js");
            
            Assert.True(resource.Assembly == GetType().Assembly);
            Assert.True(resource.Content.Length > 0);
        }
    }
}
