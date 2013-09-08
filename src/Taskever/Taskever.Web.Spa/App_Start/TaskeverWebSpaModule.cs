using Abp.Modules;
using Taskever.Web.Dependency.Installers;

namespace Taskever.Web.App_Start    
{
    [AbpModule("Taskever.Web.Spa", Dependencies = new[] { "Taskever.Web" })]
    public class TaskeverWebSpaModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new TaskeverWebSpaInstaller());
        }
    }
}