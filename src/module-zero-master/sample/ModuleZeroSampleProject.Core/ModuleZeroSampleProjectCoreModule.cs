using System.Reflection;
using Abp.Modules;
using Abp.Zero;

namespace ModuleZeroSampleProject
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class ModuleZeroSampleProjectCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
