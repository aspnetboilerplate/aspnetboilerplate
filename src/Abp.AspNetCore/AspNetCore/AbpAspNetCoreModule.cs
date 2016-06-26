using System.Reflection;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Web;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Abp.AspNetCore
{
    [DependsOn(typeof (AbpWebCommonModule))]
    public class AbpAspNetCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpAspNetCoreConfiguration, AbpAspNetCoreConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            var configuration = IocManager.Resolve<AbpAspNetCoreConfiguration>();
            var partManager = IocManager.Resolve<ApplicationPartManager>();

            foreach (var controllerAssembly in configuration.ControllerAssemblies)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(controllerAssembly));
            }
        }
    }
}