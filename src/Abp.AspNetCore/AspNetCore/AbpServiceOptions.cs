using System.Collections.Generic;
using Abp.Dependency;
using Abp.PlugIns;

namespace Abp.AspNetCore
{
    public class AbpServiceOptions
    {
        public IIocManager IocManager { get; set; }

        public List<PlugInFolderInfo> PlugInFolders { get; }

        public AbpServiceOptions()
        {
            PlugInFolders = new List<PlugInFolderInfo>();
        }
    }
}