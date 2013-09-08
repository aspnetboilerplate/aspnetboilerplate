using System.Collections.Generic;
using Abp.Utils.Extensions;

namespace Abp.Modules.Loading
{
    /// <summary>
    /// This class is used to get a sorted list of modules according to their dependencies to each other.
    /// </summary>
    internal class AbpModuleDependencySorter
    {
        /// <summary>
        /// Sorts modules accorting to dependencies.
        /// </summary>
        /// <param name="modules">All modules</param>
        /// <returns>Sorted list</returns>
        public List<AbpModuleInfo> SortByDependency(IDictionary<string, AbpModuleInfo> modules)
        {
            var orderedList = new List<AbpModuleInfo>();

            foreach (var module in modules.Values)
            {
                var index = 0; //Order of this module (will be first module if there is no dependencies of it)

                //Check all modules and place this module after all it's dependencies
                if (!module.Dependencies.IsNullOrEmpty())
                {
                    for (int i = 0; i < orderedList.Count; i++)
                    {
                        //Check for dependency
                        if (module.Dependencies.ContainsKey(orderedList[i].Name))
                        {
                            //If there is dependency, place after it
                            index = i + 1;
                        }
                    }
                }

                //Insert module the right place in the list
                orderedList.Insert(index, module);
            }

            return orderedList;
        }
    }
}
