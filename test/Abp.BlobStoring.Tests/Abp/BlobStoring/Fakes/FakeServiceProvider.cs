using Castle.Windsor;
using System;

namespace Abp.BlobStoring.Tests.Abp.BlobStoring.Fakes;

public class FakeServiceProvider : IServiceProvider
{
    private static IWindsorContainer _container;

    public FakeServiceProvider(IWindsorContainer container)
    {
        _container = container;
    }

    public object GetService(Type serviceType)
    {
        return _container.Resolve(serviceType);
    }
}