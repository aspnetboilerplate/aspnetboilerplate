using System;
using System.Collections.Generic;
using System.Linq;

namespace Abp.PlugIns
{
    public class PlugInListSource : IPlugInSource
    {
        private readonly Type[] _moduleTypes;

        public PlugInListSource(params Type[] moduleTypes)
        {
            _moduleTypes = moduleTypes;
        }

        public List<Type> GetModules()
        {
            return _moduleTypes.ToList();
        }
    }
}