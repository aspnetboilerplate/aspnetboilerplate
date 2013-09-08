using Abp.Modules;
using Abp.Web.Controllers.Dynamic;
using Taskever.Services;
using Taskever.Web.Dependency.Installers;

namespace Taskever.Web.Startup
{
    [AbpModule("Taskever.Web", Dependencies = new[] { "Taskever.Core", "Taskever.Data", "Abp.Web" })]
    public class TaskeverWebModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new TaskeverWebInstaller());
            
            DynamicControllerGenerator.GenerateFor<ITaskService>();
        }
    }
}