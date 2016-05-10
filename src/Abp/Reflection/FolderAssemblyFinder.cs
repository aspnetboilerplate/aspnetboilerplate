using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Abp.Reflection
{
    public class FolderAssemblyFinder : IAssemblyFinder
    {
        private readonly object _syncLock = new object();

        private List<Assembly> _assemblies;

        public FolderAssemblyFinder(string folderPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            FolderPath = folderPath;
            SearchOption = searchOption;
        }

        public string FolderPath { get; }

        public SearchOption SearchOption { get; }

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

            foreach (var dllFile in dllFiles)
            {
                assemblies.Add(Assembly.LoadFile(dllFile));
            }

            return assemblies;
        }
    }
}