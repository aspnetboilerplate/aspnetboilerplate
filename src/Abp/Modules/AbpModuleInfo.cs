using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abp.Modules
{
    /// <summary>
    ///     Used to store all needed information for a module.
    /// </summary>
    internal class AbpModuleInfo
    {
        /// <summary>
        ///     Creates a new AbpModuleInfo object.
        /// </summary>
        /// <param name="instance"></param>
        public AbpModuleInfo(AbpModule instance)
        {
            Dependencies = new List<AbpModuleInfo>();
            Type = instance.GetType();
            Instance = instance;
            Assembly = Type.Assembly;
        }

        /// <summary>
        ///     The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        ///     Type of the module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Instance of the module.
        /// </summary>
        public AbpModule Instance { get; private set; }

        /// <summary>
        ///     All dependent modules of this module.
        /// </summary>
        public List<AbpModuleInfo> Dependencies { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", Type.AssemblyQualifiedName);
        }
    }
}