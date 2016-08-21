using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Web.Security;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Abp.WebApi.Security;
using Abp.WebApi.Validation;
using Castle.Core.Logging;

namespace Abp.WebApi.Authorization
{
    public class AbpApiCsrfFilter : IAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public bool AllowMultiple => false;

        private readonly ICsrfTokenManager _csrfManager;

        public AbpApiCsrfFilter(ICsrfTokenManager csrfManager)
        {
            _csrfManager = csrfManager;

            Logger = NullLogger.Instance;
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

            if (!_csrfManager.ShouldValidate(methodInfo, actionContext.Request.Method.ToHttpVerb(), true))
            {
                return await continuation();
            }

            if (!_csrfManager.IsValid(actionContext.Request.Headers))
            {
                return CreateForbiddenResponse(actionContext, "A request done with an empty or invalid CSRF header token. It should be same of the Cookie value!");
            }
            
            return await continuation();
        }

        protected virtual HttpResponseMessage CreateForbiddenResponse(HttpActionContext actionContext, string reason)
        {
            Logger.Warn(reason);
            Logger.Warn("Requested URI: " + actionContext.Request.RequestUri);
            return new HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = reason };
        }
    }
}