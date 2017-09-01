using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFrameworkCore;

namespace Abp.Zero.SampleApp.EntityFrameworkCore
{
    [DependsOn(typeof(AbpZeroEntityFrameworkCoreModule), typeof(SampleAppModule))]
    public class SampleAppEntityFrameworkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
