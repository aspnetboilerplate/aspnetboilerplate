using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Dependency;

namespace Abp.WebApi.Validation
{
    public class AbpValidationFilterAttribute : IActionFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IIocResolver _iocResolver;

        public AbpValidationFilterAttribute(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo != null)
            {
                using (var validator = _iocResolver.ResolveAsDisposable<WebApiActionInvocationValidator>())
                {
                    validator.Object.Initialize(actionContext, methodInfo);
                    validator.Object.Validate();
                }
            }

            return await continuation();
        }
    }
}