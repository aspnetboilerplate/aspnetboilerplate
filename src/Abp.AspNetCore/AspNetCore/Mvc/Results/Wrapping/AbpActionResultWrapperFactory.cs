using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public static class AbpActionResultWrapperFactory
    {
        public static IAbpActionResultWrapper CreateFor([NotNull] ResultExecutingContext actionResult)
        {
            Check.NotNull(actionResult, nameof(actionResult));

            if (actionResult.Result is ObjectResult)
            {
                return new AbpObjectActionResultWrapper();
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