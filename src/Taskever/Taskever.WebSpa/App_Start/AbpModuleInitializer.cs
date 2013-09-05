using Abp.Modules.Core;
using Abp.Web.Modules;
using Taskever.Web.App_Start;

[assembly: WebActivator.PreApplicationStartMethod(typeof(AbpModuleRegistrer), "RegisterModules")]

namespace Taskever.Web.App_Start
{
    /// <summary>
    /// TODO: Is that needed or scan all assemblies?
    /// </summary>
    public static class AbpModuleRegistrer
    {
        public static void RegisterModules()
        {
            AbpModuleManager.Instance.RegisterModule(new AbpCoreModule());
            AbpModuleManager.Instance.RegisterModule(new TaskeverModule());
        }
    }
}