using System.Reflection;
using Abp.Modules;

namespace Abp.AspNetCore
{
    public class AbpAspNetCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}