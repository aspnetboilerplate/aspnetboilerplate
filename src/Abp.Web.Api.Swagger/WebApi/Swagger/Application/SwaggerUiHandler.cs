using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Abp.WebApi.Swagger.Ui;

namespace Abp.WebApi.Swagger.Application
{
    /// <summary>
    /// Swagger Ui Handler;
    /// We can use this class to resolve requested resource and output to browser.
    /// </summary>
    public class AbpSwaggerUiHandler : HttpMessageHandler
    {
        private readonly AbpSwaggerUiConfig _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Swagger Ui Configuration</param>
        public AbpSwaggerUiHandler(AbpSwaggerUiConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation
        /// </summary>
        /// <param name="request">The HTTP request message to send</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var swaggerUiProvider = _config.GetSwaggerUiProvider();
            var rootUrl = _config.GetRootUrl(request);

            //get resource path from current http request message.
            var assetPath = request.GetRouteData().Values["assetPath"].ToString();

            try
            {
                //Get embedded resource use Asset to wrap it. 
                var webAsset = swaggerUiProvider.GetAsset(rootUrl, assetPath);
                //Get HttpContent
                var content = ContentFor(webAsset);

                return TaskFor(new HttpResponseMessage { Content = content });

            }
            catch (AssetNotFound ex)
            {
                return TaskFor(request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
            }
        }

        /// <summary>
        /// Use embedded resource to fill HttpContent and set MediaType
        /// </summary>
        /// <param name="webAsset">Embedded resource</param>
        /// <returns></returns>
        private HttpContent ContentFor(Asset webAsset)
        {
            var content = new StreamContent(webAsset.Stream);

            content.Headers.ContentType = new MediaTypeHeaderValue(webAsset.MediaType);

            return content;
        }

        private Task<HttpResponseMessage> TaskFor(HttpResponseMessage response)
        {
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }
    }
}
