using System.Linq;
using System.Reflection;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Auditing;
using Abp.AspNetCore.Runtime.Session;
using Abp.AspNetCore.Security.AntiForgery;
using Abp.Auditing;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Runtime.Session;
using Abp.Web;
using Abp.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

namespace Abp.AspNetCore
{
    [DependsOn(typeof(AbpWebCommonModule))]
    public class AbpAspNetCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpAspNetCoreConfiguration, AbpAspNetCoreConfiguration>();

            Configuration.ReplaceService<IPrincipalAccessor, AspNetCorePrincipalAccessor>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IAbpAntiForgeryManager, AbpAspNetCoreAntiForgeryManager>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IClientInfoProvider, HttpContextClientInfoProvider>(DependencyLifeStyle.Transient);

            Configuration.Modules.AbpAspNetCore().FormBodyBindingIgnoredTypes.Add(typeof(IFormFile));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            AddApplicationParts();
            ConfigureAntiforgery();
        }

        private void AddApplicationParts()
        {
            var configuration = IocManager.Resolve<AbpAspNetCoreConfiguration>();
            var partManager = IocManager.Resolve<ApplicationPartManager>();
            var moduleManager = IocManager.Resolve<IAbpModuleManager>();

            var controllerAssemblies = configuration.ControllerAssemblySettings.Select(s => s.Assembly).Distinct();
            foreach (var controllerAssembly in controllerAssemblies)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(controllerAssembly));
            }

            var plugInAssemblies = moduleManager.Modules.Where(m => m.IsLoadedAsPlugIn).Select(m => m.Assembly).Distinct();
            foreach (var plugInAssembly in plugInAssemblies)
            {
                partManager.ApplicationParts.Add(new AssemblyPart(plugInAssembly));
            }
        }

        private void ConfigureAntiforgery()
        {
            IocManager.Using<IOptions<AntiforgeryOptions>>(optionsAccessor =>
            {
                optionsAccessor.Value.HeaderName = Configuration.Modules.AbpWebCommon().AntiForgery.TokenHeaderName;
            });
        }
    }
}