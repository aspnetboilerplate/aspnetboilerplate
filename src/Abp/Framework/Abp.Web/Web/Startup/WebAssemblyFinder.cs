using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Abp.Modules;

namespace Abp.Web.Startup
{
    public class WebAssemblyFinder : IAssemblyFinder
    {
        public List<Assembly> GetAllAssemblies()
        {
            var allAssemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();

            var binFolder = HttpRuntime.AppDomainAppPath + "bin\\";
            var dllFiles = Directory.GetFiles(binFolder, "*.dll", SearchOption.TopDirectoryOnly).ToList();

            var binAssemblies = new List<Assembly>();
            foreach (string dllFile in dllFiles)
            {
                var assemblyName = AssemblyName.GetAssemblyName(dllFile);
                var locatedAssembly = allAssemblies.FirstOrDefault(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), assemblyName));
                if (locatedAssembly != null)
                {
                    binAssemblies.Add(locatedAssembly);
                }
            }

            return binAssemblies;
        }
    }
}
