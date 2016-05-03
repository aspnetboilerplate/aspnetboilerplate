using Abp.MemoryDb.Configuration;
using Abp.Modules;
using System.Reflection;

namespace Abp.MemoryDb
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