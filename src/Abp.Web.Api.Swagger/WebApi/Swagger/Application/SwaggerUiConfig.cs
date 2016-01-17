using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Abp.WebApi.Swagger.Builders;
using Abp.WebApi.Swagger.Ui;
using Newtonsoft.Json;

namespace Abp.WebApi.Swagger.Application
{
    /// <summary>
    /// Configuration Swagger UI
    /// </summary>
    public class AbpSwaggerUiConfig
    {
        private readonly Dictionary<string, EmbeddedAssetDescriptor> _pathToAssetMap;
        private readonly Dictionary<string, string> _templateParams;
        private readonly Func<HttpRequestMessage, string> _rootUrlResolver;

        /// <summary>
        /// AbpSwaggerUiConfig Constructor
        /// </summary>
        /// <param name="rootUrlResolver">According HTTP request message to get root url</param>
        public AbpSwaggerUiConfig(Func<HttpRequestMessage, string> rootUrlResolver)
        {
            _pathToAssetMap = new Dictionary<string, EmbeddedAssetDescriptor>();

            _templateParams = new Dictionary<string, string>
            {
                {"%(StylesheetIncludes)", ""},
                {"%(TranslatorIncludes)", ""},
                {"%(DiscoveryPaths)", ""},
                {"%(BooleanValues)", "true|false"},
                {"%(ValidatorUrl)", ""},
                {"%(CustomScripts)", ""},
                {"%(DocExpansion)", "none"},
                {"%(OAuth2Enabled)", "false"},
                {"%(OAuth2ClientId)", ""},
                {"%(OAuth2ClientSecret)", ""},
                {"%(OAuth2Realm)", ""},
                {"%(OAuth2AppName)", ""},
                {"%(AbpSwaggerModel)", ""}
            };

            _rootUrlResolver = rootUrlResolver;

            MapPathsForSwaggerUiAssets();

            // Use some custom versions to support config and extensionless paths
            var thisAssembly = GetType().Assembly;

            CustomAsset("index", thisAssembly, "Abp.WebApi.Swagger.Ui.CustomAssets.index.html");
            CustomAsset("lib/swagger-oauth-js", thisAssembly, "Abp.WebApi.Swagger.Ui.CustomAssets.swagger-oauth.js");
        }

        /// <summary>
        /// <para>
        /// Use the "InjectStylesheet" option to enrich the UI with one or more additional CSS stylesheets.
        /// </para>
        /// <para>
        /// The file must be included in your project as an "Embedded Resource", and then the resource's
        /// </para>
        /// <para>
        /// "Logical Name" is passed to the method as shown below.
        /// </para>
        /// </summary>
        /// <param name="resourceAssembly">This assembly of your webapp</param>
        /// <param name="resourceName">your style name</param>
        /// <param name="media">indicating your document shows to which device;(screen, print, all)</param>
        public void InjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen")
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(StylesheetIncludes)"]);
            stringBuilder.AppendLine("<link href='" + path + "' media='" + media +
                                     "' rel='stylesheet' type='text/css' />");
            _templateParams["%(StylesheetIncludes)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName);
        }

        /// <summary>
        /// There is some language resources to pick.
        /// eg : en, es, fr, it, ja, pl, pt, ru, tr, zh-cn.
        /// </summary>
        /// <param name="lang"></param>
        public void EnableTranslator(string lang = "en")
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script src='lang/translator-js' type='text/javascript'></script>");
            sb.AppendLine("<script src='lang/"+ lang +"-js' type='text/javascript'></script>");
            _templateParams["%(TranslatorIncludes)"] = sb.ToString();
        }

        /// <summary>
        /// <para>
        /// The swagger-ui renders boolean data types as a dropdown. By default, it provides "true" and "false"
        /// </para>
        /// <para>
        /// strings as the possible choices. You can use this option to change these to something else,
        /// </para>
        /// <para>
        /// eg : 0 and 1.
        /// </para>
        /// </summary>
        /// <param name="values"></param>
        public void BooleanValues(IEnumerable<string> values)
        {
            _templateParams["%(BooleanValues)"] = string.Join("|", values);
        }

        /// <summary>
        /// By default, swagger-ui will validate specs against swagger.io's online validator and display the result
        /// in a badge at the bottom of the page. Use these options to set a different validator URL or to disable the
        /// feature entirely.
        /// </summary>
        /// <param name="url"></param>
        public void SetValidatorUrl(string url)
        {
            _templateParams["%(ValidatorUrl)"] = url;
        }

        /// <summary>
        /// By default, swagger-ui will validate specs against swagger.io's online validator and display the result
        /// in a badge at the bottom of the page. Use these options to set a different validator URL or to disable the
        /// feature entirely.
        /// </summary>
        public void DisableValidator()
        {
            _templateParams["%(ValidatorUrl)"] = "null";
        }

        /// <summary>
        /// Use the "InjectJavaScript" option to invoke one or more custom JavaScripts after the swagger-ui
        /// has loaded. The file must be included in your project as an "Embedded Resource", and then the resource's
        /// "Logical Name" is passed to the method as shown above.
        /// </summary>
        /// <param name="resourceAssembly"></param>
        /// <param name="resourceName"></param>
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

        /// <summary>
        /// Use this option to control how the Operation listing is displayed.
        /// It can be set to "None" (default), "List" (shows operations for each resource),
        /// or "Full" (fully expanded: shows operations and their details).
        /// </summary>
        /// <param name="docExpansion">None, List, Full.</param>
        public void DocExpansion(DocExpansion docExpansion)
        {
            _templateParams["%(DocExpansion)"] = docExpansion.ToString().ToLower();
        }

        /// <summary>
        /// Use the CustomAsset option to provide your own version of assets used in the swagger-ui.
        /// It's typically used to instruct Swashbuckle to return your version instead of the default
        /// when a request is made for "index.html". As with all custom content, the file must be included
        /// in your project as an "Embedded Resource", and then the resource's "Logical Name" is passed to
        /// the method as shown below.
        /// eg : CustomAsset("index", containingAssembly, "YourWebApiProject.SwaggerExtensions.index.html");
        /// </summary> 
        /// <param name="path">your resource name</param>
        /// <param name="resourceAssembly">your webapp assembly</param>
        /// <param name="resourceName">resource's "Logical Name"</param>
        public void CustomAsset(string path, Assembly resourceAssembly, string resourceName)
        {
            _pathToAssetMap[path] = new EmbeddedAssetDescriptor(resourceAssembly, resourceName, path == "index");
        }
        
        /// <summary>
        /// If your API supports the OAuth2 Implicit flow, and you've described it correctly, according to
        /// the Swagger 2.0 specification, you can enable UI support as shown below.</summary>
        /// <param name="clientId"></param>
        /// <param name="realm"></param>
        /// <param name="appName"></param>
        public void EnableOAuth2Support(string clientId, string realm, string appName)
        {
            EnableOAuth2Support(clientId, "N/A", realm, appName);
        }

        /// <summary>
        /// If your API supports the OAuth2 Implicit flow, and you've described it correctly, according to
        /// the Swagger 2.0 specification, you can enable UI support as shown below.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="realm"></param>
        /// <param name="appName"></param>
        public void EnableOAuth2Support(string clientId, string clientSecret, string realm, string appName)
        {
            _templateParams["%(OAuth2Enabled)"] = "true";
            _templateParams["%(OAuth2ClientId)"] = clientId;
            _templateParams["%(OAuth2ClientSecret)"] = clientSecret;
            _templateParams["%(OAuth2Realm)"] = realm;
            _templateParams["%(OAuth2AppName)"] = appName;
        }

        internal void SetAbpSwaggerModel(AbpSwaggerModel model)
        {
            var strJson = JsonConvert.SerializeObject(model);
            _templateParams["%(AbpSwaggerModel)"] = strJson;
        }

        /// <summary>
        /// Get asset provider for swagger ui
        /// </summary>
        /// <returns></returns>
        internal IAssetProvider GetSwaggerUiProvider()
        {
            return new EmbeddedAssetProvider(_pathToAssetMap, _templateParams);
        }

        /// <summary>
        /// get request root url
        /// </summary>
        /// <param name="swaggerRequest">The HTTP request message</param>
        /// <returns></returns>
        internal string GetRootUrl(HttpRequestMessage swaggerRequest)
        {
            return _rootUrlResolver(swaggerRequest);
        }

        /// <summary>
        /// Map resource path to assets of swagger ui
        /// </summary>
        private void MapPathsForSwaggerUiAssets()
        {
            var thisAssembly = GetType().Assembly;
            foreach (var resourceName in thisAssembly.GetManifestResourceNames())
            {
                if (resourceName.Contains("Abp.WebApi.Swagger.Ui.CustomAssets")) continue; // original assets only

                var path = resourceName
                    .Replace("\\", "/")
                    .Replace(".", "-"); // extensionless to avoid RUMMFAR

                _pathToAssetMap[path] = new EmbeddedAssetDescriptor(thisAssembly, resourceName, path == "index");
            }
        }
    }

    /// <summary>
    ///  Use this option to control how the Operation listing is displayed.
    /// </summary>
    public enum DocExpansion
    {
        /// <summary>
        /// default, resource is shrinking
        /// </summary>
        None,
        /// <summary>
        /// shows operations for each resource
        /// </summary>
        List,
        /// <summary>
        /// fully expanded: shows operations and their details
        /// </summary>
        Full
    }
}
