using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;

namespace Abp.Zero.SampleApp.EntityFramework
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(SampleAppModule))]
    public class SampleAppEntityFrameworkModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
