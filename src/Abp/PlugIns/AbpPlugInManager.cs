using System.Collections.Generic;
using System.Reflection;
using Abp.Reflection;

namespace Abp.PlugIns
{
    public class AbpPlugInManager : IAbpPlugInManager
    {
        public List<PlugInFolderInfo> PlugInFolders
        {
            get { return _plugInFolders; }
            set
            {
                Check.NotNull(value, nameof(value));
                _plugInFolders = value;
            }
        }
        private List<PlugInFolderInfo> _plugInFolders;

        public AbpPlugInManager()
        {
            _plugInFolders = new List<PlugInFolderInfo>();
        }

        public List<Assembly> GetPlugInAssemblies()
        {
            var plugInAssemblies = new List<Assembly>();

            if (PlugInFolders == null)
            {
                return plugInAssemblies;
            }

            foreach (var plugInFolderInfo in PlugInFolders)
            {
                plugInAssemblies.AddRange(
                    AssemblyHelper.GetAllAssembliesInFolder(
                        plugInFolderInfo.Folder,
                        plugInFolderInfo.SearchOption
                    )
                );
            }

            return plugInAssemblies;
        }
    }
}