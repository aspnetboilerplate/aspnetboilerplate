using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;

namespace Abp.BlobStoring.Tests.Abp.BlobStoring.Fakes;

public class FakeProviders : ISingletonDependency
{
    public FakeBlobProvider1 Provider1 { get; }

    public FakeBlobProvider2 Provider2 { get; }

    public FakeProviders(IocManager iocManager)
    {
        var providers = iocManager.ResolveAll<IBlobProvider>();
        Provider1 = providers.OfType<FakeBlobProvider1>().Single();
        Provider2 = providers.OfType<FakeBlobProvider2>().Single();
    }
}