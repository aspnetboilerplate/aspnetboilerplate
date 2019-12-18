using System;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpObjectActionResultWrapper : IAbpActionResultWrapper
    {
        public void Wrap(FilterContext context)
        {
            ObjectResult objectResult = null;

            switch (context)
            {
                case ResultExecutingContext resultExecutingContext:
                    objectResult = resultExecutingContext.Result as ObjectResult;
                    break;

                case PageHandlerExecutedContext pageHandlerExecutedContext:
                    objectResult = pageHandlerExecutedContext.Result as ObjectResult;
                    break;
            }

            if (objectResult == null)
            {
                throw new ArgumentException("Action Result should be JsonResult!");
            }

            if (!(objectResult.Value is AjaxResponseBase))
            {
                objectResult.Value = new AjaxResponse(objectResult.Value);
                objectResult.DeclaredType = typeof(AjaxResponse);
            }
        }
    }
}