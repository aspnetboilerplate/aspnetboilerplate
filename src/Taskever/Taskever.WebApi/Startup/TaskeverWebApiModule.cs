using Abp.Modules;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Taskever.Services;
using Taskever.Web.Dependency.Installers;

namespace Taskever.Web.Startup
{
    [AbpModule("Taskever.WebApi", Dependencies = new[] { "Taskever.Data", "Abp.WebApi" })]
    public class TaskeverWebApiModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new TaskeverWebInstaller());
            CreateWebApiProxiesForServices();
        }

        private void CreateWebApiProxiesForServices()
        {
            BuildApiController
                .For<ITaskService>()//.WithControllerName("Tasking")
                .ForMethod("DeleteTask").WithActionName("DeleteTask").WithVerb(HttpVerb.Get)
                .Build();
        }
    }
}