using System.Reflection;
using Abp.Modules;
using Abp.WebApi.Swagger.Configuration;

namespace Abp.WebApi.Swagger
{
    public class AbpSwaggerModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpSwaggerModuleConfiguration, AbpSwaggerModuleConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
