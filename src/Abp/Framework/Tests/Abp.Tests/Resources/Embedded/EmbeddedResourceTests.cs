using Abp.Resources.Embedded;
using Abp.Utils.Extensions;
using NUnit.Framework;

namespace Abp.Tests.Resources.Embedded
{
    [TestFixture]
    public class EmbeddedResourceTests
    {
        private IEmbeddedResourceManager _embeddedResourceManager;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _embeddedResourceManager = new EmbeddedResourceManager();
        }

        [Test]
        public void Should_Define_And_Get_Embedded_Resources()
        {
            _embeddedResourceManager.ExposeResources("MyApp/MyResources", GetType().Assembly, "Abp.Tests.Resources.Embedded.MyResources");
            var resource = _embeddedResourceManager.GetResource("MyApp/MyResources/js/MyScriptFile1.js");
            Assert.True(resource.Assembly == GetType().Assembly);
            Assert.True(resource.Content.Length > 0);
        }
    }
}
