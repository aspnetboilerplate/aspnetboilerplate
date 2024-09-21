using System;
using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Configuration;

public class AbpEfCoreConfiguration : IAbpEfCoreConfiguration
{
    private readonly IIocManager _iocManager;

    public AbpEfCoreConfiguration(IIocManager iocManager)
    {
        _iocManager = iocManager;
    }

    public bool UseAbpQueryCompiler { get; set; } = false;

    public void AddDbContext<TDbContext>(Action<AbpDbContextConfiguration<TDbContext>> action)
        where TDbContext : DbContext
    {
        _iocManager.IocContainer.Register(
            Component.For<IAbpDbContextConfigurer<TDbContext>>().Instance(
                new AbpDbContextConfigurerAction<TDbContext>(action)
            ).IsDefault()
        );
    }
}