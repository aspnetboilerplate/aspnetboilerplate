using System;
using System.Collections.Generic;

namespace Abp.Modules
{
    /// <summary>
    /// Used to store all needed informations for a module.
    /// </summary>
    internal class AbpModuleInfo
    {
        /// <summary>
        /// Unique Name of the module. Shortcut for <see cref="AbpModuleAttribute.Name"/>.
        /// </summary>
        public string Name { get { return ModuleAttribute.Name; } }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public IAbpModule Instance { get; private set; }

        /// <summary>
        /// Declared AbpModuleAttribute attribute for this module.
        /// </summary>
        public AbpModuleAttribute ModuleAttribute { get; private set; }
        
        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public IDictionary<string, AbpModuleInfo> Dependencies { get; private set; }
        
        public AbpModuleInfo(Type type, AbpModuleAttribute moduleAttribute, IAbpModule instance)
        {
            Dependencies = new Dictionary<string, AbpModuleInfo>();
            Type = type;
            ModuleAttribute = moduleAttribute;
            Instance = instance;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, Type.FullName);
        }
    }
}