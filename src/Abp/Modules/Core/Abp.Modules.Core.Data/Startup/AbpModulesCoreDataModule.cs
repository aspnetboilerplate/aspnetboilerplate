using System.Reflection;
using Abp.Data.Startup;
using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Abp.Modules.Core.Startup.Dependency;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.Data", Dependencies = new[] { "Abp.Data", "Abp.Modules.Core" })]
    public class AbpModulesCoreDataModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            initializationContext.GetModule<AbpDataModule>().AddMapping(
                m => m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(UserMap)))
                ); //TODO: Remove this to Core.Data and remove fluent nhibernate dependency?
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreDataModuleDependencyInstaller());
        }
    }
}
