using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if !NET46
using System.Runtime.Loader;
#endif

namespace Abp.Reflection
{
    internal static class AssemblyHelper
    {
        public static List<Assembly> GetAllAssembliesInFolder(string folderPath, SearchOption searchOption)
        {
            var assemblyFiles = Directory
                .EnumerateFiles(folderPath, "*.*", searchOption)
                .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));

            return assemblyFiles.Select(
#if NET46
                Assembly.LoadFile
#else
                AssemblyLoadContext.Default.LoadFromAssemblyPath
#endif
                ).ToList();
        }
    }
}
