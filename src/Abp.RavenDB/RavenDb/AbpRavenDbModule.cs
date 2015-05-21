using System.Reflection;
using Abp.Modules;
using Abp.RavenDb.Configuration;

namespace Abp.RavenDb
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in RavenDB.
    /// </summary>
    public class AbpRavenDbModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpRavenDbModuleConfiguration, AbpRavenDbModuleConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
