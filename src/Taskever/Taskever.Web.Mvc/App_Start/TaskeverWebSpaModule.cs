using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;

namespace Taskever.Web.Mvc    
{
    [AbpModule("Taskever.Web.Spa", Dependencies = new[] { "Abp.Web.Mvc", "Taskever.Web.Api" })]
    public class TaskeverWebSpaModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}