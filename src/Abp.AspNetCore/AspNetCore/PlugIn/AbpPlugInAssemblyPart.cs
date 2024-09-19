using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Abp.AspNetCore.PlugIn;

public class AbpPlugInAssemblyPart : AssemblyPart, ICompilationReferencesProvider
{
    public AbpPlugInAssemblyPart(Assembly assembly)
        : base(assembly)
    {
    }

    IEnumerable<string> ICompilationReferencesProvider.GetReferencePaths() => Enumerable.Empty<string>();
}