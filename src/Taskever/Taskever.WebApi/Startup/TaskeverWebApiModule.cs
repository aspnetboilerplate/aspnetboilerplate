using Abp.Modules;
using Abp.WebApi.Controllers.Dynamic;
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
            
            DynamicApiControllerGenerator.GenerateFor<ITaskService>();
        }
    }
}