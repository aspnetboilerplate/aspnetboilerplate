using System.Reflection;
using Abp.Modules;
using Abp.Web;

namespace Abp.AspNetCore
{
    [DependsOn(typeof (AbpWebCommonModule))]
    public class AbpAspNetCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}