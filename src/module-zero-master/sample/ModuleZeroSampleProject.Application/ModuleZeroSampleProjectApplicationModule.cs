using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using ModuleZeroSampleProject.Authorization;
using ModuleZeroSampleProject.Configuration;

namespace ModuleZeroSampleProject
{
    [DependsOn(typeof(ModuleZeroSampleProjectCoreModule), typeof(AbpAutoMapperModule))]
    public class ModuleZeroSampleProjectApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Configuration.Authorization.Providers.Add<ModuleZeroSampleProjectAuthorizationProvider>();
            Configuration.Settings.Providers.Add<MySettingProvider>();
        }
    }
}
