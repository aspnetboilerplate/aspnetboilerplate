using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpActionResultWrapperFactory : IAbpActionResultWrapperFactory
    {
        public IAbpActionResultWrapper CreateFor(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            switch (context)
            {
                case ResultExecutingContext resultExecutingContext when resultExecutingContext.Result is ObjectResult:
                    return new AbpObjectActionResultWrapper();

                case ResultExecutingContext resultExecutingContext when resultExecutingContext.Result is JsonResult:
                    return new AbpJsonActionResultWrapper();

                case ResultExecutingContext resultExecutingContext when resultExecutingContext.Result is EmptyResult:
                    return new AbpEmptyActionResultWrapper();

                case PageHandlerExecutedContext pageHandlerExecutedContext when pageHandlerExecutedContext.Result is ObjectResult:
                    return new AbpObjectActionResultWrapper();

                case PageHandlerExecutedContext pageHandlerExecutedContext when pageHandlerExecutedContext.Result is JsonResult:
                    return new AbpJsonActionResultWrapper();

                case PageHandlerExecutedContext pageHandlerExecutedContext when pageHandlerExecutedContext.Result is EmptyResult:
                    return new AbpEmptyActionResultWrapper();

                default:
                    return new NullAbpActionResultWrapper();
            }
        }
    }
}