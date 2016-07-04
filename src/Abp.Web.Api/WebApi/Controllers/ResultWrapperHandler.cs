using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Web.Models;
using Abp.WebApi.Configuration;

namespace Abp.WebApi.Controllers
{
    /// <summary>
    /// Wraps Web API return values by <see cref="AjaxResponse"/>.
    /// </summary>
    public class ResultWrapperHandler : DelegatingHandler, ITransientDependency
    {
        private readonly IAbpWebApiModuleConfiguration _webApiModuleConfiguration;

        public ResultWrapperHandler(IAbpWebApiModuleConfiguration webApiModuleConfiguration)
        {
            _webApiModuleConfiguration = webApiModuleConfiguration;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
                task =>
                {
                    WrapResultIfNeeded(request, task.Result);
                    return task.Result;

                }, cancellationToken);
        }

        protected virtual void WrapResultIfNeeded(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            var wrapAttr = HttpActionDescriptorHelper.GetWrapResultAttributeOrNull(request.GetActionDescriptor())
                           ?? DontWrapResultAttribute.Default;

            if (!wrapAttr.WrapOnSuccess)
            {
                return;
            }

            object resultObject;
            if (!response.TryGetContentValue(out resultObject) || resultObject == null)
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new ObjectContent<AjaxResponse>(
                    new AjaxResponse(),
                    _webApiModuleConfiguration.HttpConfiguration.Formatters.JsonFormatter
                    );
                return;
            }

            if (resultObject is AjaxResponse)
            {
                return;
            }

            response.Content = new ObjectContent<AjaxResponse>(
                new AjaxResponse(resultObject),
                _webApiModuleConfiguration.HttpConfiguration.Formatters.JsonFormatter
                );
        }
    }
}