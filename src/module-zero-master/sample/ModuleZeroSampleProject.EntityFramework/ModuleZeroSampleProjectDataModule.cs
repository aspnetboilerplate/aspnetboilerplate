using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;

namespace ModuleZeroSampleProject
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(ModuleZeroSampleProjectCoreModule))]
    public class ModuleZeroSampleProjectDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
