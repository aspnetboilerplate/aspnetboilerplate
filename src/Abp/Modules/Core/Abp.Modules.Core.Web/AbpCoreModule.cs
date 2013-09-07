using System.Reflection;
using Abp.Data;
using Abp.Modules.Core.Entities.NHibernate.Mappings;
using Abp.Startup;
using Castle.Windsor.Installer;

namespace Abp.Modules.Core
{
    [AbpModule("Abp.Modules.Core", Dependencies = new [] { "Abp.Data" })]
    public class AbpCoreModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            initializationContext.GetModule<AbpDataModule>().AddMapping(m => m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(UserMap)))); //TODO: Remove this to Core.Data and remove fluent nhibernate dependency?
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            initializationContext.IocContainer.Install(FromAssembly.This());

            AutoMappingManager.Map();
        }
    }
}
