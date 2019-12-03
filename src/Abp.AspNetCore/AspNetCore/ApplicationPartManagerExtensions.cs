using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Abp.AspNetCore
{
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
    }
}
