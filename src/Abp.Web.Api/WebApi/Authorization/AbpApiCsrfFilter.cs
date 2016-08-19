using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Reflection;
using Abp.Web.Security;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Abp.WebApi.Validation;
using Castle.Core.Logging;

namespace Abp.WebApi.Authorization
{
    public class AbpApiCsrfFilter : IAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public bool AllowMultiple => false;

        private readonly ICsrfConfiguration _configuration;

        public AbpApiCsrfFilter(ICsrfConfiguration configuration)
        {
            _configuration = configuration;

            Logger = NullLogger.Instance;
        }

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            if (!_configuration.IsEnabled)
            {
                return await continuation();
            }

            if (_configuration.IgnoredHttpVerbs.Contains(actionContext.Request.Method.ToHttpVerb()))
            {
                return await continuation();
            }

            var methodInfo = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return await continuation();
            }

            if (!methodInfo.IsDefined(typeof(ValidateCsrfTokenAttribute), true) && 
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableCsrfTokenValidationAttribute>(methodInfo) != null)
            {
                return await continuation();
            }

            var csrfCookie = actionContext.Request.Headers.GetCookies(_configuration.TokenCookieName).LastOrDefault();
            if (csrfCookie == null)
            {
                return await continuation();
            }

            var cookieTokenValue = csrfCookie[_configuration.TokenCookieName].Value;
            if (cookieTokenValue.IsNullOrEmpty())
            {
                return await continuation();
            }

            IEnumerable<string> tokenHeaderValues;
            if (!actionContext.Request.Headers.TryGetValues(_configuration.TokenHeaderName, out tokenHeaderValues))
            {
                return CreateForbiddenResponse(actionContext, "A request done with an empty CSRF header token but with non-empty CSRF cookie value!");
            }

            var headerTokenValue = tokenHeaderValues.Last();
            if (headerTokenValue != cookieTokenValue)
            {
                return CreateForbiddenResponse(actionContext, "A request done with an invalid CSRF header token. It should be same of the Cookie value!");
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