using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.BlobStoring.Tests.Abp.BlobStoring.Fakes;

public class FakeServiceScopeFactory : IServiceScopeFactory
{
    private readonly IWindsorContainer _container;

    public FakeServiceScopeFactory(IWindsorContainer container)
    {
        _container = container;
    }

    public IServiceScope CreateScope()
    {
        return new WindsorServiceScope(_container, new MsLifetimeScope(_container));
    }
}