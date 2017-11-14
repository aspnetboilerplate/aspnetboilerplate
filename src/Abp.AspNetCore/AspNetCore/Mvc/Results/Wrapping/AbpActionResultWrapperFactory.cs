using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpActionResultWrapperFactory : IAbpActionResultWrapperFactory
    {
        public IAbpActionResultWrapper CreateFor(ResultExecutingContext actionResult)
        {
            Check.NotNull(actionResult, nameof(actionResult));

            if (actionResult.Result is ObjectResult)
            {
                return new AbpObjectActionResultWrapper(actionResult.HttpContext.RequestServices);
            }

            if (actionResult.Result is JsonResult)
            {
                return new AbpJsonActionResultWrapper();
            }

            if (actionResult.Result is EmptyResult)
            {
                return new AbpEmptyActionResultWrapper();
            }

            return new NullAbpActionResultWrapper();
        }
    }
}