using System;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Repositories;
using Abp.EntityFrameworkCore.Configuration;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.Application.Shop;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Abp.ZeroCore.SampleApp.Core.Shop;
using Abp.ZeroCore.SampleApp.EntityFramework;
using Abp.ZeroCore.SampleApp.EntityFramework.Seed;
using AutoMapper;
using Castle.MicroKernel.Registration;

namespace Abp.ZeroCore.SampleApp;

[DependsOn(typeof(AbpZeroCoreEntityFrameworkCoreModule), typeof(AbpAutoMapperModule))]
public class AbpZeroCoreSampleAppModule : AbpModule
{
    /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
    public bool SkipDbContextRegistration { get; set; }

    public override void PreInitialize()
    {
        if (!SkipDbContextRegistration)
        {
            Configuration.Modules.AbpEfCore().AddDbContext<SampleAppDbContext>(configuration =>
            {
                AbpZeroTemplateDbContextConfigurer.Configure(configuration.DbContextOptions,
                    configuration.ConnectionString);
            });
        }

        Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

        Configuration.Features.Providers.Add<AppFeatureProvider>();

        Configuration.CustomConfigProviders.Add(new TestCustomConfigProvider());
        Configuration.CustomConfigProviders.Add(new TestCustomConfigProvider2());

        Configuration.Modules.AbpEfCore().UseAbpQueryCompiler = true;
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(AbpZeroCoreSampleAppModule).GetAssembly());

        var genericRepositoryRegistarar = IocManager.Resolve<EfGenericRepositoryRegistrar>();

        genericRepositoryRegistarar.RegisterForEntity(
            typeof(SampleAppDbContext),
            typeof(CustomEntity),
            IocManager,
            EfCoreAutoRepositoryTypes.Default
        );

        genericRepositoryRegistarar.RegisterForEntity(
            typeof(SampleAppDbContext),
            typeof(CustomEntityWithGuidId),
            IocManager,
            EfCoreAutoRepositoryTypes.Default
        );

        Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
        {
            CustomDtoMapper.CreateMappings(configuration, new MultiLingualMapContext(
                IocManager.Resolve<ISettingManager>()
            ));
        });
    }

    public override void PostInitialize()
    {
        SeedHelper.SeedHostDb(IocManager);
    }
}

internal static class CustomDtoMapper
{
    public static void CreateMappings(IMapperConfigurationExpression configuration, MultiLingualMapContext context)
    {
        // Product
        configuration.CreateMultiLingualMap<Product, ProductTranslation, ProductListDto>(context, true);
        configuration.CreateMap<ProductCreateDto, Product>();
        configuration.CreateMap<ProductUpdateDto, Product>();
        configuration.CreateMap<ProductTranslationDto, ProductTranslation>();

        // Office
        configuration.CreateMultiLingualMap<Office, int, OfficeTranslation, long, OfficeListDto>(context, true);
        configuration.CreateMap<OfficeCreateDto, Office>();
        configuration.CreateMap<OfficeUpdateDto, Office>();
        configuration.CreateMap<OfficeTranslationDto, OfficeTranslation>();

        configuration.CreateMultiLingualMap<Order, OrderTranslation, OrderListDto>(context, true)
            .EntityMap.ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
    }
}