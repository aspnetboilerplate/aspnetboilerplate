using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Abp.Exceptions;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers.Filters
{
    public class AbpExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            //TODO: Move this to another class to be able to share!
            var message = context.Exception is AbpUserFriendlyException
                              ? context.Exception.Message
                              : "General exception message here!";

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.OK,
                new AbpAjaxResult(new AbpErrorInfo(message))
                );
        }
    }
}