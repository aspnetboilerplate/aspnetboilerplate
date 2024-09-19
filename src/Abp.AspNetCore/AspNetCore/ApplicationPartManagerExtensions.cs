using System.Linq;
using System.Reflection;
using Abp.AspNetCore.PlugIn;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Razor.Hosting;

namespace Abp.AspNetCore;

public static class ApplicationPartManagerExtensions
{
    public static void AddApplicationPartsIfNotAddedBefore(this ApplicationPartManager partManager, Assembly assembly)
    {
        if (partManager.ApplicationParts.OfType<AssemblyPart>().Any(ap => ap.Assembly == assembly))
        {
            return;
        }

        partManager.ApplicationParts.Add(new AssemblyPart(assembly));
    }

    public static void AddAbpPlugInAssemblyPartIfNotAddedBefore(this ApplicationPartManager partManager, AbpPlugInAssemblyPart assemblyPart)
    {
        if (partManager.ApplicationParts.OfType<AssemblyPart>().Any(ap => ap == assemblyPart))
        {
            return;
        }

        partManager.ApplicationParts.Add(assemblyPart);
        if (assemblyPart.Assembly.GetCustomAttributes<RazorCompiledItemAttribute>().Any())
        {
            partManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(assemblyPart.Assembly));
        }
    }
}