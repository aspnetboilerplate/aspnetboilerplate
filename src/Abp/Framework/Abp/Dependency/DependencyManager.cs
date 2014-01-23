using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Dependency
{
    public static class DependencyManager
    {
        private static readonly List<IConventionalRegisterer> _registerers;

        static DependencyManager()
        {
            _registerers = new List<IConventionalRegisterer>();
        }

        public static void AddConventionalRegisterer(IConventionalRegisterer registerer)
        {
            _registerers.Add(registerer);
        }

        public static void RegisterAllByConvension()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var registerer in _registerers)
                {
                    registerer.Register(assembly);
                }
            }
        }
    }
}
