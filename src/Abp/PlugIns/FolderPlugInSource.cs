using System;
using System.Collections.Generic;
using System.IO;
using Abp.Modules;
using Abp.Reflection;

namespace Abp.PlugIns
{
    public class FolderPlugInSource : IPlugInSource
    {
        public string Folder { get; }

        public SearchOption SearchOption { get; set; }

        public FolderPlugInSource(string folder, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Folder = folder;
            SearchOption = searchOption;
        }

        public List<Type> GetModules()
        {
            var modules = new List<Type>();

            var assemblies = AssemblyHelper.GetAllAssembliesInFolder(Folder, SearchOption);
            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (AbpModule.IsAbpModule(type) || !modules.Contains(type))
                        {
                            modules.Add(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new AbpInitializationException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            return modules;
        }
    }
}