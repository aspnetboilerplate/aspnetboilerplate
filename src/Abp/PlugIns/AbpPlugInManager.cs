using System.Collections.Generic;

namespace Abp.PlugIns
{
    public class AbpPlugInManager : IAbpPlugInManager
    {
        public List<IPlugInSource> PlugInSources { get; }

        public AbpPlugInManager()
        {
            PlugInSources = new List<IPlugInSource>();
        }
    }
}