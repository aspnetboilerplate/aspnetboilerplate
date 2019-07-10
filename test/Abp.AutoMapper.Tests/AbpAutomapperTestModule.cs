using Abp.Modules;

namespace Abp.AutoMapper.Tests
{
    [DependsOn(typeof(AbpAutoMapperModule))]
    public class AbpAutoMapperTestModule : AbpModule
    {
    }
}
