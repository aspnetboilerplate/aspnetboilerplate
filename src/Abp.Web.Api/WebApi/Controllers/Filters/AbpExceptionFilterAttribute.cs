using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Logging;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers.Filters
{
    /// <summary>
    /// Used to handle exceptions on web api controllers.
    /// </summary>
    public class AbpExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            LogHelper.LogException(context.Exception);

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.OK,
                new AjaxResponse(ErrorInfoBuilder.Instance.BuildForException(context.Exception))
                );

            EventBus.Default.Trigger(this, new AbpHandledExceptionData(context.Exception));
        }
    }
}