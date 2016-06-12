using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace AbpAspNetCoreDemo.Core
{
    [DependsOn(typeof(AbpAutoMapperModule))]
    public class AbpAspNetCoreDemoCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}