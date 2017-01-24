using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Modules;

namespace Abp.PlugIns
{
    public class FileListPlugInSource : IPlugInSource
    {
        public string[] FilePaths { get; }

        public FileListPlugInSource(params string[] filePaths)
        {
            FilePaths = filePaths ?? new string[0];
        }

        public List<Type> GetModules()
        {
            var modules = new List<Type>();

            foreach (var filePath in FilePaths)
            {
                var assembly = Assembly.LoadFile(filePath);

                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (AbpModule.IsAbpModule(type))
                        {
                            modules.AddIfNotContains(type);
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