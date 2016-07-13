using System.Collections.Generic;

namespace Abp.PlugIns
{
    public interface IAbpPlugInManager
    {
        List<IPlugInSource> PlugInSources { get; }
    }
}
