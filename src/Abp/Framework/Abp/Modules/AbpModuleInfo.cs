using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Exceptions;
using Abp.Utils.Extensions.Reflection;

namespace Abp.Modules
{
    /// <summary>
    /// Used to store all needed informations for a module.
    /// </summary>
    public class AbpModuleInfo
    {
        /// <summary>
        /// Unique Name of the module. Shortcut for <see cref="AbpModuleAttribute.Name"/>.
        /// </summary>
        public string Name { get { return Instance.GetType().Name; } }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public IAbpModule Instance { get; private set; }

        public Assembly Assembly { get; private set; }
        
        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<AbpModuleInfo> Dependencies { get; private set; }
        
        public AbpModuleInfo(Type type, IAbpModule instance)
        {
            Dependencies = new List<AbpModuleInfo>();
            Type = type;
            Instance = instance;
            Assembly = type.Assembly;
        }

        public static AbpModuleInfo CreateForType(Type type)
        {
            if (!AbpModuleHelper.IsAbpModule(type))
            {
                throw new AbpException(
                    string.Format(
                        "type {0} is not an Abp module. An Abp module must be subclass of AbpModule, must declare AbpModuleAttribute attribute and must not be abstract!",
                        type.FullName));
            }

            return new AbpModuleInfo(type, (IAbpModule)Activator.CreateInstance(type, new object[] { }));
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, Type.FullName);
        }
    }
}