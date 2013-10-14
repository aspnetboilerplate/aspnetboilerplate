using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Abp.Exceptions;
using Abp.Localization;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers.Filters
{
    public class AbpExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            /* Handle exceptions for:
             * - User friendly exceptions
             * - Validation exceptions
             * - Others (as internal server error)
             */

            //TODO: Move this to another class to be able to share!
            var message = context.Exception is AbpUserFriendlyException
                              ? context.Exception.Message
                              : LocalizationHelper.GetString("InternalServerError");

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.OK,
                new AbpAjaxResponse(new AbpErrorInfo(message))
                );
        }
    }
}