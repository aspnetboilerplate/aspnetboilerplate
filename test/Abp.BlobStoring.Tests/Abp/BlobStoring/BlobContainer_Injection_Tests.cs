using Abp.BlobStoring.Tests.Abp.BlobStoring.TestObjects;
using Shouldly;
using Xunit;

namespace Abp.BlobStoring.Tests.Abp.BlobStoring;

public class BlobContainer_Injection_Tests : AbpBlobStoringTestBase
{
    [Fact]
    public void Should_Inject_DefaultContainer_For_Non_Generic_Interface()
    {
        Resolve<IBlobContainer>()
            .ShouldBeOfType<BlobContainer<DefaultContainer>>();
    }

    [Fact]
    public void Should_Inject_Specified_Container_For_Generic_Interface()
    {
        Resolve<IBlobContainer<DefaultContainer>>()
            .ShouldBeOfType<BlobContainer<DefaultContainer>>();

        Resolve<IBlobContainer<TestContainer1>>()
            .ShouldBeOfType<BlobContainer<TestContainer1>>();

        Resolve<IBlobContainer<TestContainer2>>()
            .ShouldBeOfType<BlobContainer<TestContainer2>>();

        Resolve<IBlobContainer<TestContainer3>>()
            .ShouldBeOfType<BlobContainer<TestContainer3>>();
    }
}