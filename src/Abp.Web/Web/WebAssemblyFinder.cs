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
    ///     This class is used to get all assemblies in bin folder of a web application.
    /// </summary>
    public class WebAssemblyFinder : IAssemblyFinder
    {
        /// <summary>
        ///     The search option used to find assemblies in bin folder.
        /// </summary>
        public static SearchOption FindAssembliesSearchOption = SearchOption.TopDirectoryOnly;
            //TODO: Make this non static and rename to SearchOption

        private readonly object _syncLock = new object();

        private List<Assembly> _assemblies;

        /// <summary>
        ///     This return all assemblies in bin folder of the web application.
        /// </summary>
        /// <returns>List of assemblies</returns>
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
            var assembliesInBinFolder = new List<Assembly>();

            var allReferencedAssemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            var dllFiles =
                Directory.GetFiles(HttpRuntime.AppDomainAppPath + "bin\\", "*.dll", FindAssembliesSearchOption).ToList();

            foreach (var dllFile in dllFiles)
            {
                var locatedAssembly =
                    allReferencedAssemblies.FirstOrDefault(
                        asm =>
                            AssemblyName.ReferenceMatchesDefinition(asm.GetName(), AssemblyName.GetAssemblyName(dllFile)));
                if (locatedAssembly != null)
                {
                    assembliesInBinFolder.Add(locatedAssembly);
                }
            }

            return assembliesInBinFolder;
        }
    }
}