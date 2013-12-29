using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Web.Models;
using Castle.Core.Logging;

namespace Abp.WebApi.Controllers.Filters
{
    public class AbpExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            //TODO: Create a LogHelper for that!
            using (var logger = IocHelper.ResolveAsDisposable<ILogger>())
            {
                logger.Object.Error(context.Exception.Message, context.Exception);
            }

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.OK,
                new AbpAjaxResponse(AbpErrorInfo.ForException(context.Exception))
                );
        }
    }
}