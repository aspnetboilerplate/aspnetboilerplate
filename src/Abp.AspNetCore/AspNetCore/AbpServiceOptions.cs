using System.Collections.Generic;
using Abp.Dependency;
using Abp.PlugIns;

namespace Abp.AspNetCore
{
    public class AbpServiceOptions
    {
        public IIocManager IocManager { get; set; }

        public List<IPlugInSource> PlugInSources { get; }

        public AbpServiceOptions()
        {
            PlugInSources = new List<IPlugInSource>();
        }
    }
}