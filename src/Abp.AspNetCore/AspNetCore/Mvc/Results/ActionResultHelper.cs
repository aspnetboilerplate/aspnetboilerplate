using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Results
{
    internal static class ActionResultHelper
    {
        public static bool IsObjectResult(Type returnType)
        {
            //Get the actual return type (unwrap Task)
            if (returnType == typeof(Task))
            {
                returnType = typeof(void);
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GenericTypeArguments[0];
            }

            if (typeof(IActionResult).IsAssignableFrom(returnType))
            {
                if (typeof(JsonResult).IsAssignableFrom(returnType) || typeof(ObjectResult).IsAssignableFrom(returnType))
                {
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}