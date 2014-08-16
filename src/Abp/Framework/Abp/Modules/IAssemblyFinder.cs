using System.Collections.Generic;
using System.Reflection;

namespace Abp.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAssemblyFinder
    {
        List<Assembly> GetAllAssemblies();
    }
}