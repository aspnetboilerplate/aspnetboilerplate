using System.Reflection;
using Abp.Data.Startup;
using Abp.Modules;
using Taskever.Dependency.Installers;

namespace Taskever.Startup
{
    [AbpModule("Taskever.Infrastructure.Data.NHibernate", Dependencies = new[] { "Abp.Modules.Core.Infrastructure.Data.NHibernate" })]
    public class TaskeverDataModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            initializationContext.GetModule<AbpDataModule>().AddMapping(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new TaskeverDataInstaller());
        }
    }
}