using System;
using System.Collections.Generic;

namespace Abp.PlugIns
{
    public interface IPlugInSource
    {
        List<Type> GetModules();
    }
}