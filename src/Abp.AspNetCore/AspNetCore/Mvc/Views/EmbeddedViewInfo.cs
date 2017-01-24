using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.IO.Extensions;
using Abp.Timing;
using Microsoft.Extensions.FileProviders;

namespace Abp.AspNetCore.Mvc.Views
{
    public class EmbeddedViewInfo
    {
        public Assembly Assembly { get; }

        public string RootNamespace { get; }

        public string RootFolder { get; }

        public EmbeddedViewInfo(Assembly assembly, string rootNamespace, string rootFolder = "/Views/")
        {
            Assembly = assembly;
            RootNamespace = rootNamespace;
            RootFolder = rootFolder;
        }

        internal void AddFiles(Dictionary<string, IFileInfo> files)
        {
            foreach (var resourceName in Assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith(RootNamespace))
                {
                    continue;
                }

                using (var stream = Assembly.GetManifestResourceStream(resourceName))
                {
                    var filePath = RootFolder + CalculateNormalizedPath(resourceName);

                    files[filePath] = new MemoryFileInfo(
                        CalculateFileName(filePath),
                        stream.GetAllBytes(),
                        Assembly.Location != null ? new FileInfo(Assembly.Location).LastWriteTime : Clock.Now
                    );
                }
            }
        }

        private string CalculateFileName(string filePath)
        {
            if (!filePath.Contains('/'))
            {
                return filePath;
            }

            return filePath.Substring(filePath.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }

        private string CalculateNormalizedPath(string resourceName)
        {
            var normalizedName = resourceName.Substring(RootNamespace.Length + 1);
            var parts = normalizedName.Split('.');
            if (parts.Length > 2)
            {
                normalizedName = parts.Take(parts.Length - 1).JoinAsString("/") + "." + parts.Last();
            }

            return normalizedName;
        }
    }
}