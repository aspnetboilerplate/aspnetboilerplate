using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Abp.Logging;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers.Filters
{
    /// <summary>
    /// Used to handle exceptions on web api controllers.
    /// </summary>
    public class AbpExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            LogHelper.LogException(context.Exception);
            context.Response = context.Request.CreateResponse(
                HttpStatusCode.OK,
                new AbpAjaxResponse(AbpErrorInfo.ForException(context.Exception))
                );
        }
    }
}