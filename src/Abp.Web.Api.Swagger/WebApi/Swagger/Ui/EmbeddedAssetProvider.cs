using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Abp.WebApi.Swagger.Ui
{
    /// <summary>
    /// Embedded asset provider
    /// </summary>
    public class EmbeddedAssetProvider : IAssetProvider
    {
        private readonly IDictionary<string, EmbeddedAssetDescriptor> _pathToAssetMap;
        private readonly IDictionary<string, string> _templateParams;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pathToAssetMap">Directory for embedded asset descriptor</param>
        /// <param name="templateParams">Directory for template params, See swashbuckleConfig from index.html </param>
        public EmbeddedAssetProvider(
            IDictionary<string, EmbeddedAssetDescriptor> pathToAssetMap,
            IDictionary<string, string> templateParams)
        {
            _pathToAssetMap = pathToAssetMap;
            _templateParams = templateParams;
        }

        /// <summary>
        /// Get embedded resource use Asset to wrap it.
        /// </summary>
        /// <param name="rootUrl">Root url from current http request message</param>
        /// <param name="path">the key of your embedded resource</param>
        /// <returns></returns>
        public Asset GetAsset(string rootUrl, string path)
        {
            if (!_pathToAssetMap.ContainsKey(path))
                throw new AssetNotFound($"Mapping not found - {path}");

            var resourceDescriptor = _pathToAssetMap[path];

            return new Asset(
                GetEmbeddedResourceStreamFor(resourceDescriptor, rootUrl),
                InferMediaTypeFrom(resourceDescriptor.Name)
            );
        }

        /// <summary>
        /// Get embedded resource as stream. if resource has template parameters we will use value(you have set) to replace these template parameters.
        /// </summary>
        /// <param name="resourceDescriptor">Embedded asset descriptor</param>
        /// <param name="rootUrl">Root url from current http request message</param>
        /// <returns></returns>
        private Stream GetEmbeddedResourceStreamFor(EmbeddedAssetDescriptor resourceDescriptor, string rootUrl)
        {
            var stream = resourceDescriptor.Assembly.GetManifestResourceStream(resourceDescriptor.Name);
            if (stream == null)
                throw new AssetNotFound($"Embedded resource not found - {resourceDescriptor.Name}");
            //if it is a template parameter, it will replace template parameter with real value.
            if (resourceDescriptor.IsTemplate)
            {
                var templateParams = _templateParams
                    .Union(new[] { new KeyValuePair<string, string>("%(RootUrl)", rootUrl) })
                    .ToDictionary(entry => entry.Key, entry => entry.Value);

                //we get resource file from manifest, resource file has some template parameters.
                //we can use this function to replace template parameters with these values you have set.
                return stream.FindAndReplace(templateParams);
            }

            return stream;
        }

        /// <summary>
        /// according extension to get mediatype of your resource.
        /// </summary>
        /// <param name="path">resource name</param>
        /// <returns></returns>
        private static string InferMediaTypeFrom(string path)
        {
            var extension = path.Split('.').Last();

            switch (extension)
            {
                case "css":
                    return "text/css";
                case "js":
                    return "text/javascript";
                case "gif":
                    return "image/gif";
                case "png":
                    return "image/png";
                case "eot":
                    return "application/vnd.ms-fontobject";
                case "woff":
                    return "application/font-woff";
                case "woff2":
                    return "application/font-woff2";
                case "otf":
                    return "application/font-sfnt"; // formerly "font/opentype"
                case "ttf":
                    return "application/font-sfnt"; // formerly "font/truetype"
                case "svg":
                    return "image/svg+xml";
                default:
                    return "text/html";
            }
        }
    }
}