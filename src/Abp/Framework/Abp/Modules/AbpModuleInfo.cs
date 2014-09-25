using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abp.Modules
{
    /// <summary>
    /// Used to store all needed informations for a module.
    /// </summary>
    public class AbpModuleInfo
    {
        /// <summary>
        /// The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public AbpModule Instance { get; private set; }

        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<AbpModuleInfo> Dependencies { get; private set; }

        /// <summary>
        /// Creates a new AbpModuleInfo object.
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
        /// Creates a <see cref="AbpModuleInfo"/> object for given ABP module type.
        /// </summary>
        /// <param name="type">Type of the ABP Module</param>
        /// <returns></returns>
        /// <exception cref="AbpException">
        /// Thrown if given type is not an ABP module type.
        /// </exception>
        public static AbpModuleInfo CreateForType(Type type)
        {
            if (!AbpModuleHelper.IsAbpModule(type))
            {
                throw new AbpException(
                    string.Format(
                        "type {0} is not an Abp module. An Abp module must be a non-abstract public subclass of AbpModule class!",
                        type.FullName));
            }

            return new AbpModuleInfo((AbpModule)Activator.CreateInstance(type, new object[] { }));
        }

        public override string ToString()
        {
            return string.Format("{0}", Type.AssemblyQualifiedName);
        }
    }
}