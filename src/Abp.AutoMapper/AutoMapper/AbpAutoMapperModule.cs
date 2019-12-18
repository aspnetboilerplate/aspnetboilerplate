using System;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection;
using AutoMapper;
using Castle.MicroKernel.Registration;

namespace Abp.AutoMapper
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpAutoMapperModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;

        public AbpAutoMapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            IocManager.Register<IAbpAutoMapperConfiguration, AbpAutoMapperConfiguration>();

            Configuration.ReplaceService<ObjectMapping.IObjectMapper, AutoMapperObjectMapper>();

            Configuration.Modules.AbpAutoMapper().Configurators.Add(CreateCoreMappings);
        }

        public override void PostInitialize()
        {
            CreateMappings();
        }

        private void CreateMappings()
        {
            Action<IMapperConfigurationExpression> configurer = configuration =>
            {
                FindAndAutoMapTypes(configuration);
                foreach (var configurator in Configuration.Modules.AbpAutoMapper().Configurators)
                {
                    configurator(configuration);
                }
            };

            var config = new MapperConfiguration(configurer);
            IocManager.IocContainer.Register(
                Component.For<IConfigurationProvider>().Instance(config).LifestyleSingleton()
            );

            var mapper = config.CreateMapper();
            IocManager.IocContainer.Register(
                Component.For<IMapper>().Instance(mapper).LifestyleSingleton()
            );
            AbpEmulateAutoMapper.Mapper = mapper;
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var types = _typeFinder.Find(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    return typeInfo.IsDefined(typeof(AutoMapAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapFromAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapToAttribute));
                }
            );

            Logger.DebugFormat("Found {0} classes define auto mapping attributes", types.Length);

            foreach (var type in types)
            {
                Logger.Debug(type.FullName);
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private void CreateCoreMappings(IMapperConfigurationExpression configuration)
        {
            var localizationContext = IocManager.Resolve<ILocalizationContext>();

            configuration.CreateMap<ILocalizableString, string>().ConvertUsing(ls => ls == null ? null : ls.Localize(localizationContext));
            configuration.CreateMap<LocalizableString, string>().ConvertUsing(ls => ls == null ? null : localizationContext.LocalizationManager.GetString(ls));
        }
    }
}
