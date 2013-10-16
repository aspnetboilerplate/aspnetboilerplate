using Abp.Localization;
using Abp.Modules;
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

            //TODO: Find a better way to get a reference to the localizationmanager. LocalizationHelper?
            var localizationManager = initializationContext.IocContainer.Resolve<ILocalizationManager>();
            localizationManager.RegisterSource(initializationContext.IocContainer.Resolve<ITaskeverLocalizationSource>());
        }
    }
}
