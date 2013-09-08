using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Modules;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Dependency;
using Abp.WebApi.Dependency.Installers;
using Abp.WebApi.Dependency.Interceptors;
using Abp.WebApi.Routing;
using Castle.Core;

namespace Abp.WebApi.Startup
{
    [AbpModule("Abp.WebApi")]
    public class AbpWebModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            initializationContext.IocContainer.Kernel.ComponentRegistered += ComponentRegistered;
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);  

            DynamicControllerGenerator.IocContainer = initializationContext.IocContainer;

            RouteConfig.Register(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AbpHttpControllerSelector(GlobalConfiguration.Configuration));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpActionSelector), new AbpApiControllerActionSelector());

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(initializationContext.IocContainer));

            initializationContext.IocContainer.Install(new AbpWebApiInstaller());
        }

        protected virtual void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if (handler.ComponentModel.Implementation.IsSubclassOf(typeof(ApiController))) //TODO: Is that right?
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AbpApiControllerInterceptor)));
            }
        }
    }
}
