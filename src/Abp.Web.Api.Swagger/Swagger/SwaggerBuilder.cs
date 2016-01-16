using System.Collections.Generic;
using Abp.WebApi.Controllers.Dynamic;
using NJsonSchema;
using System.Web.Http;
using System.Net.Http;
using System.Globalization;
using System.Linq;
using NSwag;

namespace Abp.Web.Api.Swagger
{

    public static class SwaggerBuilder
    {
        static string UrlPrix = "api/services";
        public static string Json = "";
        public static void EnableSwaggerJson()
        {
           var dynamicapiinfos= DynamicApiControllerManager.GetAll();
           var generator = new DynamicApiToSwaggerGenerator(UrlPrix, new JsonSchemaGeneratorSettings());

            var swaggerservice = new SwaggerService();

            //info
            swaggerservice.Info = new SwaggerInfo() { Title = "Abp DynamicApi" };

            //tag
            swaggerservice.Tags = new List<SwaggerTag>();

            foreach (var dynamicapiinfo in dynamicapiinfos)
            {             
                generator.Generate(swaggerservice, dynamicapiinfo);              
            }

            Json = swaggerservice.ToJson().ToLower();
        }
 
        public static void EnableSwaggerUI(this HttpConfiguration httpconfig)
        {
            var config = new SwaggerUiConfig(null, DefaultRootUrlResolver);           
            httpconfig.Routes.MapHttpRoute(
                name: "swagger_ui",
                routeTemplate: "dynamicapi/ui/{*assetPath}",
                defaults: null,
                constraints: new { assetPath = @".+" },
                handler: new SwaggerUiHandler(config)
            );
        }

        private static string DefaultRootUrlResolver(HttpRequestMessage request)
        {
            var scheme = GetHeaderValue(request, "X-Forwarded-Proto") ?? request.RequestUri.Scheme;
            var host = GetHeaderValue(request, "X-Forwarded-Host") ?? request.RequestUri.Host;
            var port = GetHeaderValue(request, "X-Forwarded-Port") ?? request.RequestUri.Port.ToString(CultureInfo.InvariantCulture);

            var httpConfiguration = request.GetConfiguration();
            var virtualPathRoot = httpConfiguration.VirtualPathRoot.TrimEnd('/');

            return string.Format("{0}://{1}:{2}{3}", scheme, host, port, virtualPathRoot);
        }
        private static string GetHeaderValue(HttpRequestMessage request, string headerName)
        {
            IEnumerable<string> list;
            return request.Headers.TryGetValues(headerName, out list) ? list.FirstOrDefault() : null;
        }
    }

}
