using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Linq;
using System.Web.Routing;
using Abp.WebApi.Swagger.Application;
using Abp.WebApi.Swagger.Builders;

namespace Abp.WebApi.Swagger.Configuration
{
    internal class AbpSwaggerEnabledConfiguration : IAbpSwaggerEnabledConfiguration
    {
        private static readonly string DefaultRouteTemplate = "AbpSwagger/ui/{*assetPath}";
        private readonly HttpConfiguration _httpConfig;
        private readonly Func<HttpRequestMessage, string> _rootUrlResolver;
        private readonly Action<AbpSwaggerUiConfig> _configure;

        public AbpSwaggerModel AbpSwaggerModel { get; set; }

        public AbpSwaggerEnabledConfiguration(
            HttpConfiguration httpConfig,
            Func<HttpRequestMessage, string> rootUrlResolver,
            Action<AbpSwaggerUiConfig> configure = null
            )
        {
            _httpConfig = httpConfig;
            _rootUrlResolver = rootUrlResolver;
            _configure = configure;
        }

        public void EnableAbpSwaggerUi()
        {
            EnableAbpSwaggerUi(DefaultRouteTemplate);
        }

        public void EnableAbpSwaggerUi(string routeTemplate)
        {
            var config = new AbpSwaggerUiConfig(_rootUrlResolver);
            if (_configure != null)
            {
                _configure(config);
            }
            else
            {
                config.DocExpansion(DocExpansion.List);
            }

            config.SetAbpSwaggerModel(AbpSwaggerModel);

            var routes = _httpConfig.Routes;

            if (routes.Any(x => x.RouteTemplate == routeTemplate))
            {
                RouteTable.Routes.Remove(RouteTable.Routes["abpswagger_ui"]);
            }

            _httpConfig.Routes.MapHttpRoute(
                   name: "abpswagger_ui",
                   routeTemplate: routeTemplate,
                   defaults: null,
                   constraints: new { assetPath = @".+" },
                   handler: new AbpSwaggerUiHandler(config)
                   );

            if (routeTemplate == DefaultRouteTemplate)
            {
                if (routes.All(x => x.RouteTemplate != "AbpSwagger"))
                {
                    _httpConfig.Routes.MapHttpRoute(
                        name: "abpswagger_ui_shortcut",
                        routeTemplate: "AbpSwagger",
                        defaults: null,
                        constraints:
                            new { uriResolution = new HttpRouteDirectionConstraint(HttpRouteDirection.UriResolution) },
                        handler: new RedirectHandler(_rootUrlResolver, "AbpSwagger/ui/index"));
                }
            }
        }
    }
}
