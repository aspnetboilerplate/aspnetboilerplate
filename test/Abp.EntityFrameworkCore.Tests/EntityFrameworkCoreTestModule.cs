using System.Reflection;
using Abp.Modules;

namespace Abp.EntityFrameworkCore.Tests
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class EntityFrameworkCoreTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}