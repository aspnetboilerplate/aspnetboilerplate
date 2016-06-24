using System;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpObjectActionResultWrapper : IAbpActionResultWrapper
    {
        public void Wrap(IActionResult actionResult)
        {
            var objectResult = actionResult as ObjectResult;
            if (objectResult == null)
            {
                throw new ArgumentException($"{nameof(actionResult)} should be ObjectResult!");
            }

            if (!(objectResult.Value is AjaxResponse))
            {
                objectResult.Value = new AjaxResponse(objectResult.Value);
            }
        }
    }
}