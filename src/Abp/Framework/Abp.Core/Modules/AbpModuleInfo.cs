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
        /// Name of the module. Shortcut for <see cref="AbpModuleAttribute.Name"/>.
        /// </summary>
        public string Name { get { return ModuleAttribute.Name; } }

        /// <summary>
        /// Singleton instance of the module.
        /// </summary>
        public IAbpModule Instance { get; set; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Declared AbpModuleAttribute attribute for this module.
        /// </summary>
        public AbpModuleAttribute ModuleAttribute { get; set; }
        
        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public IDictionary<string, AbpModuleInfo> Dependencies { get; private set; } //TODO: This must be read only from outside of Abp.

        /// <summary>
        /// Creates a new AbpModuleInfo object.
        /// </summary>
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