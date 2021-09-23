using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Localization;
using Abp.Logging;
using Abp.Web;
using Abp.Web.Configuration;
using Abp.Web.Models;
using Abp.WebApi.Configuration;
using Abp.WebApi.Controllers;
using Abp.WebApi.Validation;

namespace Abp.WebApi.Authorization
{
    public class AbpApiAuthorizeFilter : IAuthorizationFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly IAbpWebApiConfiguration _configuration;
        private readonly ILocalizationManager _localizationManager;
        private readonly IEventBus _eventBus;
        private readonly IAbpWebCommonModuleConfiguration _abpWebCommonModuleConfiguration;

        public AbpApiAuthorizeFilter(
            IAuthorizationHelper authorizationHelper, 
            IAbpWebApiConfiguration configuration,
            ILocalizationManager localizationManager,
            IEventBus eventBus,
            IAbpWebCommonModuleConfiguration abpWebCommonModuleConfiguration)
        {
            _authorizationHelper = authorizationHelper;
            _configuration = configuration;
            _localizationManager = localizationManager;
            _eventBus = eventBus;
            _abpWebCommonModuleConfiguration = abpWebCommonModuleConfiguration;
        }

        public virtual async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return await continuation();
            }
            
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return await continuation();
            }

            if (actionContext.ActionDescriptor.IsDynamicAbpAction())
            {
                return await continuation();
            }

            try
            {
                await _authorizationHelper.AuthorizeAsync(methodInfo, methodInfo.DeclaringType);
                return await continuation();
            }
            catch (AbpAuthorizationException ex)
            {
                LogHelper.Logger.Warn(ex.ToString(), ex);
                await _eventBus.TriggerAsync(this, new AbpHandledExceptionData(ex));
                return CreateUnAuthorizedResponse(actionContext);
            }
        }

        protected virtual HttpResponseMessage CreateUnAuthorizedResponse(HttpActionContext actionContext)
        {
            var statusCode = GetUnAuthorizedStatusCode(actionContext);

            HttpResponseMessage HandleError(bool wrap)
            {
                if (!wrap)
                {
                    return new HttpResponseMessage(statusCode);
                }

                return new HttpResponseMessage(statusCode)
                {
                    Content = new ObjectContent<AjaxResponse>(
                        new AjaxResponse(
                            GetUnAuthorizedErrorMessage(statusCode),
                            true
                        ),
                        _configuration.HttpConfiguration.Formatters.JsonFormatter
                    )
                };
            }

            var displayUrl = actionContext.Request.RequestUri.AbsolutePath;
            if (_abpWebCommonModuleConfiguration.WrapResultFilters.HasFilterForWrapOnError(displayUrl, out var wrapOnError))
            {
                return HandleError(wrapOnError);
            }
            
            var wrapResultAttribute =
                HttpActionDescriptorHelper.GetWrapResultAttributeOrNull(actionContext.ActionDescriptor) ??
                _configuration.DefaultWrapResultAttribute;

            return HandleError(wrapResultAttribute.WrapOnError);
        }

        private ErrorInfo GetUnAuthorizedErrorMessage(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.Forbidden)
            {
                return new ErrorInfo(
                    _localizationManager.GetString(AbpWebConsts.LocalizaionSourceName, "DefaultError403"),
                    _localizationManager.GetString(AbpWebConsts.LocalizaionSourceName, "DefaultErrorDetail403")
                );
            }

            return new ErrorInfo(
                _localizationManager.GetString(AbpWebConsts.LocalizaionSourceName, "DefaultError401"),
                _localizationManager.GetString(AbpWebConsts.LocalizaionSourceName, "DefaultErrorDetail401")
            );
        }

        private static HttpStatusCode GetUnAuthorizedStatusCode(HttpActionContext actionContext)
        {
            return (actionContext.RequestContext.Principal?.Identity?.IsAuthenticated ?? false)
                ? HttpStatusCode.Forbidden
                : HttpStatusCode.Unauthorized;
        }
    }
}
