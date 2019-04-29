using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpEmptyActionResultWrapper : IAbpActionResultWrapper
    {
        public void Wrap(FilterContext context)
        {
            switch (context)
            {
                case ResultExecutingContext resultExecutingContext:
                    resultExecutingContext.Result = new ObjectResult(new AjaxResponse());
                    return;

                case PageHandlerExecutedContext pageHandlerExecutedContext:
                    pageHandlerExecutedContext.Result = new ObjectResult(new AjaxResponse());
                    return;
            }
        }
    }
}