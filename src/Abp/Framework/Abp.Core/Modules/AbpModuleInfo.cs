using System;
using System.Collections.Generic;

namespace Abp.Modules
{
    public class AbpModuleInfo
    {
        public string Name { get { return ModuleAttribute.Name; } }

        public AbpModule Instance { get; set; }

        public Type Type { get; set; }

        public AbpModuleAttribute ModuleAttribute { get; set; }
        
        public IDictionary<string, AbpModuleInfo> Dependencies { get; private set; } //TODO: This must be read only from outside of Abp.

        public AbpModuleInfo()
        {
            Dependencies = new Dictionary<string, AbpModuleInfo>();
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, Type.FullName);
        }
    }
}