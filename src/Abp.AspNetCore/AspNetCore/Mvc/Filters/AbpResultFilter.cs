using Abp.AspNetCore.Mvc.Extensions;
using Abp.Reflection;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Filters
{
    public class AbpResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrNull<WrapResultAttribute>(methodInfo) ??
                WrapResultAttribute.Default;

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            var objectResult = context.Result as ObjectResult;
            if (objectResult != null)
            {
                if (!(objectResult.Value is AjaxResponse))
                {
                    objectResult.Value = new AjaxResponse(objectResult.Value);
                }

                return;
            }

            //TODO: TEST
            var jsonResult = context.Result as JsonResult;
            if (jsonResult != null)
            {
                if (!(jsonResult.Value is AjaxResponse))
                {
                    jsonResult.Value = new AjaxResponse(jsonResult.Value);
                }

                return;
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
    }
}
