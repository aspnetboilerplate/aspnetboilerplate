using System.Reflection;

using Abp.Modules;

namespace Abp.Dapper.Tests
{
    [DependsOn(typeof(AbpDapperModule))]
    public class AbpDapperTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
