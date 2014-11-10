using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Abp.Reflection;

namespace Abp.Web
{
    /// <summary>
    /// This class is used to get all assemblies in bin folder of a web application.
    /// </summary>
    public class WebAssemblyFinder : IAssemblyFinder
    {
        private readonly IAssemblyFilter _AssemblyFilter;

        /// <summary>
        /// Class constructor
        /// </summary>
        public WebAssemblyFinder(IAssemblyFilter aAssemblyFilter)
        {
            _AssemblyFilter = aAssemblyFilter;
        }
        
        /// <summary>
        /// This return all assemblies in bin folder of the web application.
        /// </summary>
        /// <returns>List of assemblies</returns>
        public List<Assembly> GetAllAssemblies()
        {
            var assembliesInBinFolder = new List<Assembly>();

            var allReferencedAssemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            var dllFiles = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "bin\\", "*.dll", SearchOption.TopDirectoryOnly).ToList();

            foreach (string dllFile in dllFiles)
            {
                var locatedAssembly = allReferencedAssemblies.FirstOrDefault(asm => AssemblyName.ReferenceMatchesDefinition(asm.GetName(), AssemblyName.GetAssemblyName(dllFile)));
                if (locatedAssembly != null)
                {
                    //Exclude any assemblies based on implementation of IAssemblyFilter
                    if (_AssemblyFilter.ExcludeAssembly(locatedAssembly.FullName))
                        continue;


                    assembliesInBinFolder.Add(locatedAssembly);
                }
            }

            return assembliesInBinFolder;
        }
    }
}
