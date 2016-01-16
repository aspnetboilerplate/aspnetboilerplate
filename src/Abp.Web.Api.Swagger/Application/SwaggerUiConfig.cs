using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace Abp.Web.Api.Swagger
{
    public class SwaggerUiConfig
    {
        private readonly Dictionary<string, EmbeddedAssetDescriptor> _pathToAssetMap;
        private readonly Dictionary<string, string> _templateParams;
        private readonly Func<HttpRequestMessage, string> _rootUrlResolver;

        public SwaggerUiConfig(IEnumerable<string> discoveryPaths, Func<HttpRequestMessage, string> rootUrlResolver)
        {
            _pathToAssetMap = new Dictionary<string, EmbeddedAssetDescriptor>();

            _templateParams = new Dictionary<string, string>
            {
               
            };
            _rootUrlResolver = rootUrlResolver;

            MapPathsForSwaggerUiAssets();

            // Use some custom versions to support config and extensionless paths
            var thisAssembly = GetType().Assembly;
            CustomAsset("index", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.index.html");
            CustomAsset("images/logo_small.png", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.images.logo_small.png");
            CustomAsset("fonts/DroidSans-Bold.ttf", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.fonts.DroidSans-Bold.ttf");
            CustomAsset("fonts/DroidSans.ttf", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.fonts.DroidSans.ttf");
            CustomAsset("css/screen.css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.css.screen.css");
            CustomAsset("css/reset.css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.css.reset.css");
            CustomAsset("css/print.css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.css.print.css");
            CustomAsset("css/style.css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.css.style.css");
            CustomAsset("css/typography.css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.css.typography.css");
            CustomAsset("lib/jquery-1.8.0.min.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery-1.8.0.min.js");
            CustomAsset("lib/jquery.slideto.min.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery.slideto.min.js");
            CustomAsset("lib/jquery.wiggle.min.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery.wiggle.min.js");
            CustomAsset("lib/jsoneditor.min.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jsoneditor.min.js");
            CustomAsset("lib/jquery.ba-bbq.min.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery.ba-bbq.min.js");
            CustomAsset("lib/handlebars-2.0.0.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.handlebars-2.0.0.js");
            CustomAsset("lib/underscore-min.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.underscore-min.js");
            CustomAsset("lib/backbone-min.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.backbone-min.js");
           
            CustomAsset("swagger-ui.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.swagger-ui.js");
            CustomAsset("lib/highlight.7.3.pack.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.highlight.7.3.pack.js");
            CustomAsset("lib/marked.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.marked.js");        
            CustomAsset("images/throbber.gif", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.images.throbber.gif"); CustomAsset("lib/swagger-oauth.js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.swagger-oauth.js");
        }



        public void CustomAsset(string path, Assembly resourceAssembly, string resourceName)
        {
            _pathToAssetMap[path] = new EmbeddedAssetDescriptor(resourceAssembly, resourceName, path == "index");
        }

  


        internal IAssetProvider GetSwaggerUiProvider()
        {
            return new EmbeddedAssetProvider(_pathToAssetMap, _templateParams);
        }

        internal string GetRootUrl(HttpRequestMessage swaggerRequest)
        {
            return _rootUrlResolver(swaggerRequest);
        }

        private void MapPathsForSwaggerUiAssets()
        {
            var thisAssembly = GetType().Assembly;
            foreach (var resourceName in thisAssembly.GetManifestResourceNames())
            {
                if (resourceName.Contains("Abp.Web.Api.Swagger.SwaggerUi.CustomAssets")) continue; // original assets only

                var path = resourceName;

                _pathToAssetMap[path] = new EmbeddedAssetDescriptor(thisAssembly, resourceName, path == "index");
            }
        }
    }


}
