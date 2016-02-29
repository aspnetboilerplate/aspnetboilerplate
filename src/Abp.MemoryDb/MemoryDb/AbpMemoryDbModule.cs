using System.Reflection;
using Adorable.MemoryDb.Configuration;
using Adorable.Modules;

namespace Adorable.MemoryDb
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in MemoryDb.
    /// </summary>
    public class AbpMemoryDbModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpMemoryDbModuleConfiguration, AbpMemoryDbModuleConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
