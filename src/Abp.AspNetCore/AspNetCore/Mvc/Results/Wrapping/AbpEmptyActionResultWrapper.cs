using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class AbpEmptyActionResultWrapper : IAbpActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            actionResult.Result = new ObjectResult(new AjaxResponse());
        }
    }
}