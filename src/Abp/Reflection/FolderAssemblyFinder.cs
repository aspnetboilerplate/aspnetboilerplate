using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Abp.Reflection
{
    public class FolderAssemblyFinder : IAssemblyFinder
    {
        public string FolderPath { get; private set; }

        public SearchOption SearchOption { get; private set; }

        private List<Assembly> _assemblies;

        private readonly object _syncLock = new object();

        public FolderAssemblyFinder(string folderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            FolderPath = folderPath;
            SearchOption = searchOption;
        }

        public List<Assembly> GetAllAssemblies()
        {
            if (_assemblies == null)
            {
                lock (_syncLock)
                {
                    if (_assemblies == null)
                    {
                        _assemblies = GetAllAssembliesInternal();
                    }
                }
            }

            return _assemblies;
        }

        private List<Assembly> GetAllAssembliesInternal()
        {
            var assemblies = new List<Assembly>();
            var dllFiles = Directory.GetFiles(FolderPath, "*.dll", SearchOption);

            foreach (string dllFile in dllFiles)
            {
                assemblies.Add(Assembly.LoadFile(dllFile));
            }

            return assemblies;
        }
    }
}