using System.Reflection;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Runtime.Session;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Runtime.Session;
using Abp.Web;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Abp.AspNetCore
{
    [DependsOn(typeof(AbpWebCommonModule))]
    public class AbpAspNetCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpAspNetCoreConfiguration, AbpAspNetCoreConfiguration>();

            Configuration.ReplaceService<IPrincipalAccessor, AspNetCorePrincipalAccessor>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            var configuration = IocManager.Resolve<AbpAspNetCoreConfiguration>();
            var partManager = IocManager.Resolve<ApplicationPartManager>();

            foreach (var controllerSetting in configuration.ServiceControllerSettings)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(controllerSetting.Assembly));
            }
        }
    }
}