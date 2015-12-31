using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Dependency;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers
{
    /// <summary>
    /// Wrapps Web API return values by <see cref="AjaxResponse"/>.
    /// </summary>
    public class ResultWrapperHandler : DelegatingHandler, ITransientDependency
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
                task =>
                {
                    WrapResultIfNeeded(request, task.Result);

                    return task.Result;

                }, cancellationToken);
        }

        private void WrapResultIfNeeded(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            var wrapAttr = HttpActionDescriptorHelper.GetWrapResultAttributeOrNull(request.GetActionDescriptor()) ?? WrapResultAttribute.Default;

            if (!wrapAttr.WrapOnSuccess)
            {
                return;
            }

            object resultObject;
            if (!response.TryGetContentValue(out resultObject))
            {
                return;
            }

            if (resultObject == null)
            {
                response.Content = new ObjectContent<AjaxResponse>(
                    new AjaxResponse(),
                    GlobalConfiguration.Configuration.Formatters[0]
                    ); //TODO: change GlobalConfiguration.Configuration.Formatters[0] !!!
            }

            if (resultObject is AjaxResponse)
            {
                return;
            }

            response.Content = new ObjectContent<AjaxResponse>(
                new AjaxResponse(resultObject),
                GlobalConfiguration.Configuration.Formatters[0]
                ); //TODO: change GlobalConfiguration.Configuration.Formatters[0] !!!
        }
    }
}