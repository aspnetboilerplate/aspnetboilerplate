using System;
using System.Linq;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.MultiTenancy;
using Abp.AspNetCore.Mvc.Auditing;
using Abp.AspNetCore.Mvc.Caching;
using Abp.AspNetCore.Runtime.Session;
using Abp.AspNetCore.Security.AntiForgery;
using Abp.AspNetCore.Webhook;
using Abp.Auditing;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Session;
using Abp.Web;
using Abp.Web.Security.AntiForgery;
using Abp.Webhooks;
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
            IocManager.AddConventionalRegistrar(new AbpAspNetCoreConventionalRegistrar());

            IocManager.Register<IAbpAspNetCoreConfiguration, AbpAspNetCoreConfiguration>();

            Configuration.ReplaceService<IPrincipalAccessor, AspNetCorePrincipalAccessor>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IAbpAntiForgeryManager, AbpAspNetCoreAntiForgeryManager>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IClientInfoProvider, HttpContextClientInfoProvider>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IWebhookSender, AspNetCoreWebhookSender>(DependencyLifeStyle.Transient);
            IocManager.Register<IGetScriptsResponsePerUserConfiguration, GetScriptsResponsePerUserConfiguration>();
            
            Configuration.Modules.AbpAspNetCore().FormBodyBindingIgnoredTypes.Add(typeof(IFormFile));

            Configuration.MultiTenancy.Resolvers.Add<DomainTenantResolveContributor>();
            Configuration.MultiTenancy.Resolvers.Add<HttpHeaderTenantResolveContributor>();
            Configuration.MultiTenancy.Resolvers.Add<HttpCookieTenantResolveContributor>();

            Configuration.Caching.Configure(GetScriptsResponsePerUserCache.CacheName, cache => { cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(30); });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpAspNetCoreModule).GetAssembly());
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

            partManager.AddApplicationPartsIfNotAddedBefore(typeof(AbpAspNetCoreModule).Assembly);

            var controllerAssemblies = configuration.ControllerAssemblySettings.Select(s => s.Assembly).Distinct();
            foreach (var controllerAssembly in controllerAssemblies)
            {
                partManager.AddApplicationPartsIfNotAddedBefore(controllerAssembly);
            }

            var plugInAssemblies = moduleManager.Modules.Where(m => m.IsLoadedAsPlugIn).Select(m => m.Assembly).Distinct();
            foreach (var plugInAssembly in plugInAssemblies)
            {
                partManager.AddApplicationPartsIfNotAddedBefore(plugInAssembly);
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