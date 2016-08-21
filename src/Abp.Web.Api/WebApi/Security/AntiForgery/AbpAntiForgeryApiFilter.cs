using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Web.Security;
using Abp.Web.Security.AntiForgery;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Abp.WebApi.Validation;
using Castle.Core.Logging;

namespace Abp.WebApi.Security.AntiForgery
{
    public class AbpAntiForgeryApiFilter : IAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public bool AllowMultiple => false;

        private readonly IAbpAntiForgeryTokenManager _abpAntiForgeryManager;

        public AbpAntiForgeryApiFilter(IAbpAntiForgeryTokenManager abpAntiForgeryManager)
        {
            _abpAntiForgeryManager = abpAntiForgeryManager;

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

            if (!_abpAntiForgeryManager.ShouldValidate(methodInfo, actionContext.Request.Method.ToHttpVerb(), true))
            {
                return await continuation();
            }

            if (!_abpAntiForgeryManager.IsValid(actionContext.Request.Headers))
            {
                return CreateForbiddenResponse(actionContext, "A request done with an empty or invalid Anti Forgery header token. It should be same of the Cookie value!");
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