using Abp.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Abp.Reflection
{
    internal static class AssemblyHelper
    {
        public static List<Assembly> GetAllAssembliesInFolder(string folderPath, SearchOption searchOption)
        {
            List<Assembly> assemblies;
            try
            {
                IEnumerable<string> assemblyFiles = Directory
                    .EnumerateFiles(folderPath, "*.*", searchOption)
                    .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));

                assemblies = assemblyFiles.Select(
                    Assembly.LoadFile
                ).ToList();
            }
            catch (Exception ex)
            {
                assemblies = new List<Assembly>();
                LogHelper.LogException(ex);
            }

            return assemblies;
        }
    }
}
