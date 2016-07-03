using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Abp.Reflection
{
    internal static class AssemblyHelper
    {
        public static List<Assembly> GetAllAssembliesInFolder(string folderPath, SearchOption searchOption)
        {
            var assemblies = new List<Assembly>();

            var assemblyFiles = Directory.GetFiles(folderPath, "*.dll|*.exe", searchOption);

            foreach (string assemblyFile in assemblyFiles)
            {
                assemblies.Add(Assembly.LoadFile(assemblyFile));
            }

            return assemblies;
        }
    }
}
