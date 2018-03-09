using Abp.Reflection.Extensions;
using Abp.Resources.Embedded;
using Shouldly;
using System.Linq;
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
                    "/MyApp/MyResources/", GetType().GetAssembly(), "Abp.Tests.Resources.Embedded.MyResources"
                )
            );

            _embeddedResourceManager = new EmbeddedResourceManager(configuration);
        }

        [Fact]
        public void Should_Define_And_Get_Embedded_Resources()
        {
            var filepath = "/MyApp/MyResources/js/MyScriptFile1.js";
            var resource = _embeddedResourceManager.GetResource(filepath);
            var filename = System.IO.Path.GetFileName(filepath);
            var extension = System.IO.Path.GetExtension(filepath);

            resource.ShouldNotBeNull();
            Assert.True(resource.Assembly == GetType().GetAssembly());
            Assert.True(resource.Content.Length > 0);
            Assert.EndsWith(filename, resource.FileName);
            Assert.True(resource.FileExtension == extension.Substring(1)); // without dot
        }

        [Fact]
        public void Should_Get_Embedded_Resource_With_Dash_In_Name()
        {
            var filepath = "/MyApp/MyResources/js/MyScriptFile-2.js";
            var resource = _embeddedResourceManager.GetResource(filepath);
            var filename = System.IO.Path.GetFileName(filepath);
            var extension = System.IO.Path.GetExtension(filepath);

            resource.ShouldNotBeNull();
            Assert.True(resource.Assembly == GetType().GetAssembly());
            Assert.True(resource.Content.Length > 0);
            Assert.EndsWith(filename, resource.FileName);
            Assert.True(resource.FileExtension == extension.Substring(1)); // without dot
        }

        [Fact]
        public void Should_Get_Embedded_Resource_With_Two_Dots_In_Name()
        {
            var filepath = "/MyApp/MyResources/js/MyScriptFile3.min.js";
            var resource = _embeddedResourceManager.GetResource(filepath);
            var filename = System.IO.Path.GetFileName(filepath);
            var extension = System.IO.Path.GetExtension(filepath);

            resource.ShouldNotBeNull();
            Assert.True(resource.Assembly == GetType().GetAssembly());
            Assert.True(resource.Content.Length > 0);
            Assert.EndsWith(filename, resource.FileName);
            Assert.True(resource.FileExtension == extension.Substring(1)); // without dot
        }

        [Fact]
        public void Should_Get_Embedded_Resource_With_Underscore_In_Name()
        {
            var filepath = "/MyApp/MyResources/js/MyScriptFile_4.js";
            var resource = _embeddedResourceManager.GetResource(filepath);
            var filename = System.IO.Path.GetFileName(filepath);
            var extension = System.IO.Path.GetExtension(filepath);
            resource.ShouldNotBeNull();
            Assert.True(resource.Assembly == GetType().GetAssembly());
            Assert.True(resource.Content.Length > 0);
            Assert.EndsWith(filename, resource.FileName);
            Assert.True(resource.FileExtension == extension.Substring(1)); // without dot
        }

        [Fact]
        public void Should_Get_Embedded_Resources_With_Dash_In_folder()
        {
            var filepath = "/MyApp/MyResources/js-dash/MyScriptFile.js";
            var resource = _embeddedResourceManager.GetResource(filepath);
            var filename = System.IO.Path.GetFileName(filepath);
            var extension = System.IO.Path.GetExtension(filepath);

            resource.ShouldNotBeNull();
            Assert.True(resource.Assembly == GetType().GetAssembly());
            Assert.True(resource.Content.Length > 0);
            Assert.EndsWith(filename, resource.FileName);
            Assert.True(resource.FileExtension == extension.Substring(1)); // without dot
        }

        [Fact]
        public void Should_Get_Embedded_Resource_With_Underscore_In_Folder()
        {
            var filepath = "/MyApp/MyResources/js_underscore/MyScriptFile.js";
            var resource = _embeddedResourceManager.GetResource(filepath);
            var filename = System.IO.Path.GetFileName(filepath);
            var extension = System.IO.Path.GetExtension(filepath);

            resource.ShouldNotBeNull();
            Assert.True(resource.Assembly == GetType().GetAssembly());
            Assert.True(resource.Content.Length > 0);
            Assert.EndsWith(filename, resource.FileName);
            Assert.True(resource.FileExtension == extension.Substring(1)); // without dot
        }

        [Fact]
        public void Should_Get_Embedded_Resources()
        {
            var filepath = "/MyApp/MyResources/js/";
            var resources = _embeddedResourceManager.GetResources(filepath);

            resources.ShouldNotBeNull();
            Assert.True(resources.Count() == 4);
        }
    }
}
