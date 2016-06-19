using System;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpJsonActionResultWrapper : IAbpActionResultWrapper
    {
        public void Wrap(IActionResult actionResult)
        {
            var jsonResult = actionResult as JsonResult;
            if (jsonResult == null)
            {
                throw new ArgumentException($"{nameof(actionResult)} should be JsonResult!");
            }

            if (!(jsonResult.Value is AjaxResponse))
            {
                jsonResult.Value = new AjaxResponse(jsonResult.Value);
            }
        }
    }
}