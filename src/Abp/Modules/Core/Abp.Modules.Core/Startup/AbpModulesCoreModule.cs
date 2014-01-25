using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Roles;
using Abp.Startup.Dependency.Installers;

namespace Abp.Startup
{
    [AbpModule("Abp.Modules.Core")]
    public class AbpModulesCoreModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetAssembly(typeof(UserRoleService)));
            initializationContext.IocContainer.Install(new AbpCoreModuleDependencyInstaller());
        }
    }
}
