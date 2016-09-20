using System.Collections.Generic;

namespace Abp.PlugIns
{
    public interface IAbpPlugInManager
    {
        PlugInSourceList PlugInSources { get; }
    }
}
