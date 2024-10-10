using System.Threading.Tasks;
using Abp.BlobStoring.Tests.Abp.BlobStoring.Fakes;
using Abp.BlobStoring.Tests.Abp.BlobStoring.TestObjects;
using NSubstitute;
using Xunit;

namespace Abp.BlobStoring.Tests.Abp.BlobStoring;

public class BlobContainerFactory_Tests : AbpBlobStoringTestBase
{
    private readonly IBlobContainerFactory _factory;
    private readonly FakeProviders _fakeProviders;

    public BlobContainerFactory_Tests()
    {
        _factory = Resolve<IBlobContainerFactory>();
        _fakeProviders = Resolve<FakeProviders>();
    }

    [Fact]
    public async Task Should_Create_Containers_With_Configured_Providers()
    {
        // TestContainer1 with FakeBlobProvider1

        await _fakeProviders.Provider1
            .DidNotReceiveWithAnyArgs()
            .ExistsAsync(default);

        await _factory
            .Create<TestContainer1>()
            .ExistsAsync("TestBlob1");

        await _fakeProviders.Provider1
            .Received(1)
            .ExistsAsync(Arg.Is<BlobProviderExistsArgs>(args =>
                    args.ContainerName == BlobContainerNameAttribute.GetContainerName<TestContainer1>() &&
                    args.BlobName == "TestBlob1"
                )
            );

        // TestContainer2 with FakeBlobProvider2

        await _fakeProviders.Provider2
            .DidNotReceiveWithAnyArgs()
            .ExistsAsync(default);

        await _factory
            .Create<TestContainer2>()
            .ExistsAsync("TestBlob2");

        await _fakeProviders.Provider2
            .Received(1)
            .ExistsAsync(Arg.Is<BlobProviderExistsArgs>(args =>
                    args.ContainerName == BlobContainerNameAttribute.GetContainerName<TestContainer2>() &&
                    args.BlobName == "TestBlob2"
                )
            );

        // TestContainer3 with FakeBlobProvider1 (default provider)

        _fakeProviders.Provider1.ClearReceivedCalls();

        await _factory
            .Create<TestContainer3>()
            .ExistsAsync("TestBlob3");

        await _fakeProviders.Provider1
            .Received(1)
            .ExistsAsync(Arg.Is<BlobProviderExistsArgs>(t =>
                    t.ContainerName == BlobContainerNameAttribute.GetContainerName<TestContainer3>() &&
                    t.BlobName == "TestBlob3"
                )
            );
    }
}