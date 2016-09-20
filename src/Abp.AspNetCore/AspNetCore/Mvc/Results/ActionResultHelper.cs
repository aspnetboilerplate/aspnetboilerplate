using System;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Results
{
    internal static class ActionResultHelper
    {
        public static bool IsObjectResult(Type returnType)
        {
            if (typeof(IActionResult).IsAssignableFrom(returnType))
            {
                if (typeof(JsonResult).IsAssignableFrom(returnType) ||
                    typeof(ObjectResult).IsAssignableFrom(returnType))
                {
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}