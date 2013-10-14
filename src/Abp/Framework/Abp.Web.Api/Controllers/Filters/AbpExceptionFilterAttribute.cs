using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers.Filters
{
    public class AbpExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = context.Request.CreateResponse(
                HttpStatusCode.OK,
                new AbpAjaxResponse(AbpErrorInfo.ForException(context.Exception))
                );
        }
    }
}