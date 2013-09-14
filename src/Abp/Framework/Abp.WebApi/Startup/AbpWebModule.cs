using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Modules;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Abp.WebApi.Dependency;
using Abp.WebApi.Dependency.Installers;
using Abp.WebApi.Dependency.Interceptors;
using Abp.WebApi.Routing;
using Castle.Core;
using Newtonsoft.Json.Serialization;

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

            BuildApiController.IocContainer = initializationContext.IocContainer;

            RouteConfig.Register(GlobalConfiguration.Configuration);

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(formatter);
            GlobalConfiguration.Configuration.Formatters.Add(new PlainTextFormatter());


            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AbpHttpControllerSelector(GlobalConfiguration.Configuration));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(initializationContext.IocContainer));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpActionSelector), new AbpApiControllerActionSelector());

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
