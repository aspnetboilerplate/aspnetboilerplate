using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Abp.Modules;
using Abp.Web.Controllers.Dynamic;
using Abp.Web.Dependency;
using Abp.Web.Dependency.Installers;
using Abp.Web.Dependency.Interceptors;
using Abp.Web.Startup;
using Castle.Core;

namespace Abp.Web
{
    [AbpModule("Abp.Web")]
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

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(initializationContext.IocContainer.Kernel));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(initializationContext.IocContainer));

            initializationContext.IocContainer.Install(new AbpWebInstaller());
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
