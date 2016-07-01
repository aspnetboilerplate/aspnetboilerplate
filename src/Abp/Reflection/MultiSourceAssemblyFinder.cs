using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;

namespace Abp.Reflection
{
    public class MultiSourceAssemblyFinder : IAssemblyFinder
    {
        public List<IAssemblyFinder> Sources { get; }

        public MultiSourceAssemblyFinder(params IAssemblyFinder[] sources)
        {
            Sources = sources == null
                ? new List<IAssemblyFinder>()
                : sources.ToList();
        }

        public List<Assembly> GetAllAssemblies()
        {
            var list = new List<Assembly>();

            foreach (var source in Sources)
            {
                var assemblies = source.GetAllAssemblies();
                foreach (var assembly in assemblies)
                {
                    list.AddIfNotContains(assembly);
                }
            }

            return list;
        }
    }
}