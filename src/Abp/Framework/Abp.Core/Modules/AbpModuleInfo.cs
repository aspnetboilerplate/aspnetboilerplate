using System;
using System.Collections.Generic;

namespace Abp.Modules
{
    public class AbpModuleInfo
    {
        public AbpModule ModuleInstance { get; set; }

        public Type ModuleType { get; set; }

        public AbpModuleAttribute ModuleAttribute { get; set; }

        public string Name { get { return ModuleAttribute.Name; } }

        public IList<AbpModuleInfo> DependentModules { get; private set; } //TODO: This must be read only from outside of Abp.

        public AbpModuleInfo()
        {
            DependentModules = new List<AbpModuleInfo>();
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, ModuleType.FullName);
        }
    }
}