using System.Collections.Generic;

namespace Abp.Modules.Loading
{
    /// <summary>
    /// 
    /// </summary>
    internal class AbpModuleDependencySorter
    {
        public List<AbpModuleInfo> SortByDependency(IDictionary<string, AbpModuleInfo> modules)
        {
            var orderedList = new List<AbpModuleInfo>();
            foreach (var module in modules.Values)
            {
                int index = 0;
                for (int i = 0; i < orderedList.Count; i++)
                {
                    if (module.Dependencies.ContainsKey(orderedList[i].Name))
                    {
                        index = i + 1;
                    }
                }

                orderedList.Insert(index, module);
            }

            return orderedList;
        }
    }
}
