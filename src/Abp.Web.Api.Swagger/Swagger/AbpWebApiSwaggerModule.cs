using System.Reflection;
using Abp.Modules;

namespace Abp.WebApi
{
    [DependsOn(typeof(AbpWebApiModule))]
    public class AbpWebApiSwaggerModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
