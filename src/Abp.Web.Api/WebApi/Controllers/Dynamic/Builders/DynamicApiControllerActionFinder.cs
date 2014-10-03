using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Application.Services;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    internal static class DynamicApiControllerActionFinder
    {
        public static List<MethodInfo> GetMethodsToBeAction<T>()
        {
            return typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method =>
                       {
                           if (method.DeclaringType == typeof(object) || method.DeclaringType == typeof(ApplicationService))
                           {
                               return false;
                           }

                           if (IsPropertyAccessor(method))
                           {
                               return false;
                           }

                           return true;
                       }).ToList();
        }

        private static bool IsPropertyAccessor(MethodInfo method)
        {
            return method.IsSpecialName && (method.Attributes & MethodAttributes.HideBySig) != 0;
        }
    }
}