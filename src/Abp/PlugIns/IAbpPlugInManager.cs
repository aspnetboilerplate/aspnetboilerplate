using System.Collections.Generic;
using System.Reflection;

namespace Abp.PlugIns
{
    public interface IAbpPlugInManager
    {
        List<PlugInFolderInfo> PlugInFolders { get; }

        List<Assembly> GetPlugInAssemblies();
    }
}
