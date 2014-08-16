using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;
using Abp.Utils.Extensions;
using Abp.Web.Mvc.Resources;
using Taskever.Startup;
using Taskever.Tasks;

namespace Taskever.Web.Mvc
{
    public class TaskeverWebMvcModule : AbpModule
    {
        public override Type[] GetDependedModules()
        {
            return new[]
                   {
                       typeof (TaskeverDataModule)
                   };
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            WebResourceHelper.ExposeEmbeddedResources("Taskever/Er/Test", typeof(TaskAppService).Assembly, "Taskever.Test");

            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}