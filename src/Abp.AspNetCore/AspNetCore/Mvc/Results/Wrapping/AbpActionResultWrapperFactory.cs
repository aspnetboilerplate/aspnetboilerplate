using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public static class AbpActionResultWrapperFactory
    {
        public static IAbpActionResultWrapper CreateFor(IActionResult actionResult)
        {
            if (actionResult is ObjectResult)
            {
                return new AbpObjectActionResultWrapper();
            }

            if (actionResult is JsonResult)
            {
                return new AbpJsonActionResultWrapper();
            }

            return new NullAbpActionResultWrapper();
        }
    }
}