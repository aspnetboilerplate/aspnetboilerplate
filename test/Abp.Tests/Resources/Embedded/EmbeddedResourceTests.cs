using Abp.Resources.Embedded;
using Shouldly;
using Xunit;

namespace Abp.Tests.Resources.Embedded
{
    public class EmbeddedResourceTests
    {
        private readonly IEmbeddedResourceManager _embeddedResourceManager;

        public EmbeddedResourceTests()
        {
            var configuration = new EmbeddedResourcesConfiguration();

            configuration.Sources.Add(
                new EmbeddedResourceSet(
                    "/MyApp/MyResources/", GetType().Assembly, "Abp.Tests.Resources.Embedded.MyResources"
                )
            );

            _embeddedResourceManager = new EmbeddedResourceManager(configuration);
        }

        [Fact]
        public void Should_Define_And_Get_Embedded_Resources()
        {
            var resource = _embeddedResourceManager.GetResource("/MyApp/MyResources/js/MyScriptFile1.js");

            resource.ShouldNotBeNull();
            Assert.True(resource.Assembly == GetType().Assembly);
            Assert.True(resource.Content.Length > 0);
        }
    }
}
