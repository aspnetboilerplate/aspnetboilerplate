using System.Collections.Generic;

namespace Abp.PlugIns
{
    public class AbpPlugInManager : IAbpPlugInManager
    {
        public PlugInSourceList PlugInSources { get; }

        public AbpPlugInManager()
        {
            PlugInSources = new PlugInSourceList();
        }
    }
}