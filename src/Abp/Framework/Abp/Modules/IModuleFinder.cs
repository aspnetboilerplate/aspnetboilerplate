using System.Collections.Generic;

namespace Abp.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModuleFinder
    {
        List<AbpModuleInfo> FindAll();
    }
}