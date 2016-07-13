using Abp.Dependency;
using Abp.PlugIns;

namespace Abp.AspNetCore
{
    public class AbpServiceOptions
    {
        public IIocManager IocManager { get; set; }

        public PlugInSourceList PlugInSources { get; }

        public AbpServiceOptions()
        {
            PlugInSources = new PlugInSourceList();
        }
    }
}