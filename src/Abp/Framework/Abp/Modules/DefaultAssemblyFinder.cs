using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultAssemblyFinder : IAssemblyFinder
    {
        public List<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }
    }
}