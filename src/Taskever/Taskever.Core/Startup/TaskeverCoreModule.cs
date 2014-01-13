using Abp.Localization;
using Abp.Modules;
using Abp.Startup;
using Taskever.Dependency.Installers;
using Taskever.Localization.Resources;

namespace Taskever.Startup
{
    [AbpModule("Taskever.Core")]
    public class TaskeverCoreModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            
            initializationContext.IocContainer.Install(new TaskeverCoreInstaller());
            initializationContext.IocContainer.Install(new EventHandlersInstaller());

            LocalizationHelper.RegisterSource<TaskeverLocalizationSource>();
        }
    }
}
