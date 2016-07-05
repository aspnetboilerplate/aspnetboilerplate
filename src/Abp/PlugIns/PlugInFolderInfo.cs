using System.IO;

namespace Abp.PlugIns
{
    public class PlugInFolderInfo
    {
        public string Folder { get; }

        public SearchOption SearchOption { get; set; }

        public PlugInFolderInfo(string folder, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Folder = folder;
            SearchOption = searchOption;
        }
    }
}