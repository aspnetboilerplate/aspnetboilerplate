using System.Reflection;
using Abp.MemoryDb.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;

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
            IocManager.RegisterAssemblyByConvention(typeof(AbpMemoryDbModule).GetAssembly());
        }
    }
}
