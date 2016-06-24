using System.Reflection;
using Abp.Modules;

namespace Abp.AutoMapper.Tests
{
    [DependsOn(typeof(AbpAutoMapperModule))]
    public class AutoMapperTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}