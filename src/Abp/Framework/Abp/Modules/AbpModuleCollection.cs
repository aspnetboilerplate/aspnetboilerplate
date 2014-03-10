using System.Collections.Generic;
using System.Linq;
using Abp.Exceptions;
using Abp.Utils.Extensions.Collections;

namespace Abp.Modules
{
    /// <summary>
    /// Used to store AbpModuleInfo objects as a dictionary.
    /// </summary>
    public class AbpModuleCollection : List<AbpModuleInfo>
    {
        /// <summary>
        /// Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Module type</typeparam>
        /// <returns>Reference to the module instance</returns>
        public TModule GetModule<TModule>() where TModule : AbpModule
        {
            var module = this.FirstOrDefault(m => m.Type == typeof(TModule));
            if (module == null)
            {
                throw new AbpException("Can not find module for " + typeof(TModule).FullName);
            }

            return (TModule)module.Instance;
        }

        /// <summary>
        /// Sorts modules accorting to dependencies.
        /// If module A depends on mobule B, A comes after B in the returned List.
        /// </summary>
        /// <returns>Sorted list</returns>
        public List<AbpModuleInfo> GetSortedModuleListByDependency()
        {
            var orderedList = new List<AbpModuleInfo>();

            foreach (var moduleInfo in this)
            {
                var index = 0; //Order of this module (will be first module if there is no dependencies of it)

                //Check all modules and place this module after all it's dependencies
                if (!moduleInfo.Dependencies.IsNullOrEmpty())
                {
                    for (var i = 0; i < orderedList.Count; i++)
                    {
                        //Check for dependency
                        if (moduleInfo.Dependencies.Contains(orderedList[i]))
                        {
                            //If there is dependency, place after it
                            index = i + 1;
                        }
                    }
                }

                //Insert module the right place in the list
                orderedList.Insert(index, moduleInfo);
            }

            return orderedList;
        }
    }
}