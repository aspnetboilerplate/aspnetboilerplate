using System.Reflection;
using System.Web;
using Abp.Dependency;
using Abp.Modules.Core.Mvc.Startup;
using Abp.Startup;

[assembly: PreApplicationStartMethod(typeof(Test), "Start")]

namespace Abp.Modules.Core.Mvc.Startup
{
    [AbpModule("Abp.Modules.Core.Web.Mvc", Dependencies = new[] { "Abp.Web.Mvc" })]
    public class AbpModulesCoreWebMvcModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

    public class Test
    {
        public static void Start()
        {
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(AbpWebMvcModule));
        }
    }

    public class AbpWebMvcModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += (sender, e) =>
            {
                var user = context.Request.RequestContext.HttpContext.User;
                if (user.Identity.IsAuthenticated && !string.IsNullOrWhiteSpace(user.Identity.Name))
                {
                    
                }
            };

            context.PreRequestHandlerExecute += (sender, args) =>
            {
                var user = context.Request.RequestContext.HttpContext.User;
                //context.Session["AbpUser"] = user.Identity.Name;
            };
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
