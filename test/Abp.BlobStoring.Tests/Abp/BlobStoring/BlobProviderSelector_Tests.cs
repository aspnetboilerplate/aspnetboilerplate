using Abp.BlobStoring.Tests.Abp.BlobStoring.Fakes;
using Abp.BlobStoring.Tests.Abp.BlobStoring.TestObjects;
using Shouldly;
using Xunit;

namespace Abp.BlobStoring.Tests.Abp.BlobStoring;

public class BlobProviderSelector_Tests : AbpBlobStoringTestBase
{
    private readonly IBlobProviderSelector _selector;

    public BlobProviderSelector_Tests()
    {
        _selector = Resolve<IBlobProviderSelector>();
    }

    [Fact]
    public void Should_Select_Default_Provider_If_Not_Configured()
    {
        _selector.Get<TestContainer3>().ShouldBeAssignableTo<FakeBlobProvider1>();
    }

    [Fact]
    public void Should_Select_Configured_Provider()
    {
        _selector.Get<TestContainer1>().ShouldBeAssignableTo<FakeBlobProvider1>();
        _selector.Get<TestContainer2>().ShouldBeAssignableTo<FakeBlobProvider2>();
    }
}