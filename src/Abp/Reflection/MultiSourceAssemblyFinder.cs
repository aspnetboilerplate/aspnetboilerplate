using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Reflection
{
    public class MultiSourceAssemblyFinder : IAssemblyFinder
    {
        public List<IAssemblyFinder> Sources { get; private set; }

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
                list.AddRange(source.GetAllAssemblies());
            }

            return list;
        }
    }
}