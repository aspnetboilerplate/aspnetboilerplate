using System.Net;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Localization;
using Abp.Web;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Abp.AspNetCore.ExceptionHandling
{
    public class AbpAuthorizationExceptionHandlingMiddleware : IMiddleware, ITransientDependency
    {
        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly ILocalizationManager _localizationManager;

        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        public AbpAuthorizationExceptionHandlingMiddleware(
            IErrorInfoBuilder errorInfoBuilder,
            ILocalizationManager localizationManager)
        {
            _errorInfoBuilder = errorInfoBuilder;
            _localizationManager = localizationManager;

            EventBus = NullEventBus.Instance;
            Logger = NullLogger.Instance;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);

            if (IsAuthorizationExceptionStatusCode(context))
            {
                var exception = new AbpAuthorizationException(GetAuthorizationExceptionMessage(context));

                Logger.Error(exception.Message);

                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(
                        new AjaxResponse(
                            _errorInfoBuilder.BuildForException(exception),
                            true
                        )
                    )
                );

                EventBus.Trigger(this, new AbpHandledExceptionData(exception));
            }
        }

        protected virtual string GetAuthorizationExceptionMessage(HttpContext context)
        {
            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                _localizationManager.GetString(AbpWebConsts.LocalizaionSourceName, "DefaultError403");
            }

            return _localizationManager.GetString(AbpWebConsts.LocalizaionSourceName, "DefaultError401");
        }

        protected virtual bool IsAuthorizationExceptionStatusCode(HttpContext context)
        {
            return context.Response.StatusCode == (int)HttpStatusCode.Forbidden
                   || context.Response.StatusCode == (int)HttpStatusCode.Unauthorized;
        }
    }
}
