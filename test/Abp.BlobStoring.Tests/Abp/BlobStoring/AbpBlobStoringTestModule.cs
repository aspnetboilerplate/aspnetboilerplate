using System;
using Abp.BlobStoring.Tests.Abp.BlobStoring.Fakes;
using Abp.BlobStoring.Tests.Abp.BlobStoring.TestObjects;
using Abp.Modules;
using Abp.TestBase;
using NSubstitute;
using Castle.MicroKernel.Registration;
using System.Reflection;
using Abp.Configuration.Startup;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Abp.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.BlobStoring.Tests.Abp.BlobStoring;

[DependsOn(
    typeof(AbpBlobStoringModule),
    typeof(AbpTestBaseModule)
)]
public class AbpBlobStoringTestModule : AbpModule
{

    public override void PreInitialize()
    {
        IocManager.IocContainer.Register(
            Component.For<ICancellationTokenProvider>().Instance(NullCancellationTokenProvider.Instance)
                .LifestyleSingleton()
        );

        IocManager.IocContainer.Register(
            Component.For<IBlobProvider>().Instance(Substitute.For<FakeBlobProvider1>())
                .LifestyleSingleton()
        );

        IocManager.IocContainer.Register(
            Component.For<IBlobProvider>().Instance(Substitute.For<FakeBlobProvider2>())
                .LifestyleSingleton()
        );

        Configuration.Modules.AbpBlobStoring().Containers.ConfigureDefault(container =>
            {
                container.SetConfiguration("TestConfigDefault", "TestValueDefault");
                container.ProviderType = typeof(FakeBlobProvider1);
            })
            .Configure<TestContainer1>(container =>
            {
                container.SetConfiguration("TestConfig1", "TestValue1");
                container.ProviderType = typeof(FakeBlobProvider1);
            })
            .Configure<TestContainer2>(container =>
            {
                container.SetConfiguration("TestConfig2", "TestValue2");
                container.ProviderType = typeof(FakeBlobProvider2);
            });

        base.PreInitialize();

    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());


        IocManager.IocContainer.Register(
            Component.For<IServiceProvider>().Instance(new FakeServiceProvider(IocManager.IocContainer))
                .LifestyleSingleton()
        );

        IocManager.IocContainer.Register(
            Component.For<IServiceScopeFactory>().Instance(new FakeServiceScopeFactory(IocManager.IocContainer))
                .LifestyleSingleton()
        );
    }
}