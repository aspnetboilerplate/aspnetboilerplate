using Abp.Localization;
using Abp.Modules;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Reflection;
using AutoMapper;
using Castle.Core.Logging;

namespace Abp.AutoMapper
{

    [DependsOn(typeof(AbpKernelModule))]
    public class AbpAutoMapperModule : AbpModule
    {
        public ILogger Logger { get; set; }

        private readonly ITypeFinder _typeFinder;

        private static bool _createdMappingsBefore;
        private static readonly object _syncObj = new object();

        public AbpAutoMapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
            Logger = NullLogger.Instance;
        }

        public override void PreInitialize()
        {
            Configuration.ReplaceService<Abp.ObjectMapping.IObjectMapper, AutoMapperObjectMapper>();
        }

        public override void PostInitialize()
        {
            CreateMappings();
        }

        private void CreateMappings()
        {
            lock (_syncObj)
            {
                //We should prevent duplicate mapping in an application, since AutoMapper is static.
                if (_createdMappingsBefore)
                {
                    return;
                }
                Mapper.Initialize(cfg =>
                {
                    FindAndAutoMapTypes(cfg);
                    CreateOtherMappings(cfg);
                });

                _createdMappingsBefore = true;
            }
        }

        private void FindAndAutoMapTypes(IConfiguration cfg)
        {
            var types = _typeFinder.Find(type =>
                type.IsDefined(typeof(AutoMapAttribute)) ||
                type.IsDefined(typeof(AutoMapFromAttribute)) ||
                type.IsDefined(typeof(AutoMapToAttribute))
                );

            Logger.DebugFormat("Found {0} classes defines auto mapping attributes", types.Length);
            foreach (var type in types)
            {
                Logger.Debug(type.FullName);
                AutoMapperHelper.CreateMap(type, cfg);
            }
        }

        private void CreateOtherMappings(IConfiguration cfg)
        {
            var localizationManager = IocManager.Resolve<ILocalizationManager>();
            cfg.CreateMap<LocalizableString, string>("AbpAutoMapperModuleProfile").ConvertUsing(ls => localizationManager.GetString(ls));
        }
    }
}
