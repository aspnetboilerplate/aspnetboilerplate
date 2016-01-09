using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                { "%(StylesheetIncludes)", "" },
                { "%(DiscoveryPaths)", String.Join("|", discoveryPaths) },
                { "%(BooleanValues)", "true|false" },
                { "%(ValidatorUrl)", "" },
                { "%(CustomScripts)", "" },
                { "%(DocExpansion)", "none" },
                { "%(OAuth2Enabled)", "false" },
                { "%(OAuth2ClientId)", "" },
                { "%(OAuth2ClientSecret)", "" },
                { "%(OAuth2Realm)", "" },
                { "%(OAuth2AppName)", "" }
            };
            _rootUrlResolver = rootUrlResolver;

            MapPathsForSwaggerUiAssets();

            // Use some custom versions to support config and extensionless paths
            var thisAssembly = GetType().Assembly;
            CustomAsset("index", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.index.html");
            CustomAsset("css/screen-css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.screen.css");
            CustomAsset("css/reset-css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.css.reset.css");
            CustomAsset("css/print-css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.css.print.css");
            CustomAsset("lib/jquery-1-8-0-min-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery-1.8.0.min.js");
            CustomAsset("lib/jquery-slideto-min-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery-slideto.min.js");
            CustomAsset("lib/jquery-wiggle-min-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery.wiggle.min.js");
            CustomAsset("lib/jquery-ba-bbq-min-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery.ba-bbq.min.js");
            CustomAsset("lib/handlebars-2-0-0-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.handlebars-2.0.0.js");
            CustomAsset("lib/underscore-min-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.underscore-min.js");
            CustomAsset("lib/backbone-min-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.backbone-min.js");
            CustomAsset("lib/jquery-slideto-min-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.jquery-slideto.min.js");
            CustomAsset("swagger-ui-min-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.swagger-ui.min.js");
            CustomAsset("lib/highlight-7-3-pack-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.highlight.7.3.pack.js");
            CustomAsset("lib/marked-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.lib.marked.js");
            CustomAsset("fonts/droid-sans-v6-latin-700-woff2", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.fonts.droid-sans-v6-latin-700.woff2");
            CustomAsset("fonts/droid-sans-v6-latin-regular-woff2", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.fonts.droid-sans-v6-latin-regular.woff2");
            CustomAsset("fonts/droid-sans-v6-latin-700-woff", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.fonts.droid-sans-v6-latin-700.woff");
            CustomAsset("fonts/droid-sans-v6-latin-regular-woff", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.fonts.droid-sans-v6-latin-regular.woff");
            CustomAsset("fonts/droid-sans-v6-latin-700-ttf", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.fonts.droid-sans-v6-latin-700.ttf");
            CustomAsset("fonts/droid-sans-v6-latin-regular-ttf", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.fonts.droid-sans-v6-latin-regular.ttf");
            CustomAsset("images/throbber-gif", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.images.throbber.gif");
            CustomAsset("css/typography-css", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.typography.css");

            CustomAsset("lib/swagger-oauth-js", thisAssembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.swagger-oauth.js");
        }

        public void InjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen")
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(StylesheetIncludes)"]);
            stringBuilder.AppendLine("<link href='" + path + "' media='" + media + "' rel='stylesheet' type='text/css' />");
            _templateParams["%(StylesheetIncludes)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName);
        }

        public void BooleanValues(IEnumerable<string> values)
        {
            _templateParams["%(BooleanValues)"] = String.Join("|", values);
        }

        public void SetValidatorUrl(string url)
        {
            _templateParams["%(ValidatorUrl)"] = url;
        }

        public void DisableValidator()
        {
            _templateParams["%(ValidatorUrl)"] = "null";
        }

        public void InjectJavaScript(Assembly resourceAssembly, string resourceName)
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(CustomScripts)"]);
            if (stringBuilder.Length > 0)
                stringBuilder.Append("|");

            stringBuilder.Append(path);
            _templateParams["%(CustomScripts)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName);
        }

        public void DocExpansion(DocExpansion docExpansion)
        {
            _templateParams["%(DocExpansion)"] = docExpansion.ToString().ToLower();
        }

        public void CustomAsset(string path, Assembly resourceAssembly, string resourceName)
        {
            _pathToAssetMap[path] = new EmbeddedAssetDescriptor(resourceAssembly, resourceName, path == "index");
        }

        public void EnableDiscoveryUrlSelector()
        {
            InjectJavaScript(GetType().Assembly, "Abp.Web.Api.Swagger.SwaggerUi.CustomAssets.discoveryUrlSelector.js");
        }

        public void EnableOAuth2Support(string clientId, string realm, string appName)
        {
            EnableOAuth2Support(clientId, "N/A", realm, appName);
        }

        public void EnableOAuth2Support(string clientId, string clientSecret, string realm, string appName)
        {
            _templateParams["%(OAuth2Enabled)"] = "true";
            _templateParams["%(OAuth2ClientId)"] = clientId;
            _templateParams["%(OAuth2ClientSecret)"] = clientSecret;
            _templateParams["%(OAuth2Realm)"] = realm;
            _templateParams["%(OAuth2AppName)"] = appName;
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

                var path = resourceName
                    .Replace("\\", "/")
                    .Replace(".", "-"); // extensionless to avoid RUMMFAR

                _pathToAssetMap[path] = new EmbeddedAssetDescriptor(thisAssembly, resourceName, path == "index");
            }
        }
    }

    public enum DocExpansion
    {
        None,
        List,
        Full
    }
}
