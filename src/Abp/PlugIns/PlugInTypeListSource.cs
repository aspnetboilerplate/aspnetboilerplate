using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.PlugIns
{
    public class PlugInTypeListSource : IPlugInSource
    {
        private readonly Type[] _moduleTypes;
        private readonly Lazy<List<Assembly>> _assemblies;

        public PlugInTypeListSource(params Type[] moduleTypes)
        {
            _moduleTypes = moduleTypes;

            _assemblies = new Lazy<List<Assembly>>(LoadAssemblies, true);
        }

        public List<Assembly> GetAssemblies()
        {
            return _assemblies.Value;
        }

        public List<Type> GetModules()
        {
            return _moduleTypes.ToList();
        }

        private List<Assembly> LoadAssemblies()
        {
            return _moduleTypes.Select(type => type.GetTypeInfo().Assembly).ToList();
        }
    }
}