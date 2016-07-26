using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Logging;
using Abp.WebApi.Validation;

namespace Abp.WebApi.Authorization
{
    public class AbpApiAuthorizeFilter : IAuthorizationFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IAuthorizationHelper _authorizationHelper;

        public AbpApiAuthorizeFilter(IAuthorizationHelper authorizationHelper)
        {
            _authorizationHelper = authorizationHelper;
        }

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
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
                await _authorizationHelper.AuthorizeAsync(methodInfo);
                return await continuation();
            }
            catch (AbpAuthorizationException ex)
            {
                LogHelper.Logger.Warn(ex.ToString(), ex);
                return CreateUnAuthorizedResponse(actionContext);
            }
        }

        private static HttpResponseMessage CreateUnAuthorizedResponse(HttpActionContext actionContext)
        {
            var response = new HttpResponseMessage(
                actionContext.RequestContext.Principal?.Identity?.IsAuthenticated ?? false
                    ? HttpStatusCode.Forbidden
                    : HttpStatusCode.Unauthorized
            );

            return response;
        }
    }
}