using System;

namespace Abp.Modules
{
    /// <summary>
    /// This attribute is used to declare properties of a module
    /// </summary>
    public class AbpModuleAttribute : Attribute
    {
        /// <summary>
        /// Name of the module.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This array stores list of dependent modules.
        /// </summary>
        public string[] Dependencies { get; set; }

        /// <summary>
        /// Creates a new AbpModuleAttribute object.
        /// </summary>
        /// <param name="name">Name of the module</param>
        public AbpModuleAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Module name can not be null or empty", "name");
            }

            Name = name;
            Dependencies = new string[] { };
        }
    }
}