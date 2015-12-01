using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Abp.Web.Api.Swagger
{
    public class EmbeddedAssetProvider : IAssetProvider
    {
        private readonly IDictionary<string, EmbeddedAssetDescriptor> _pathToAssetMap;
        private readonly IDictionary<string, string> _templateParams;

        public EmbeddedAssetProvider(
            IDictionary<string, EmbeddedAssetDescriptor> pathToAssetMap,
            IDictionary<string, string> templateParams)
        {
            _pathToAssetMap = pathToAssetMap;
            _templateParams = templateParams;
        }

        public Asset GetAsset(string rootUrl, string path)
        {
            if (!_pathToAssetMap.ContainsKey(path))
                throw new AssetNotFound(String.Format("Mapping not found - {0}", path));

            var resourceDescriptor = _pathToAssetMap[path];
            return new Asset(
                GetEmbeddedResourceStreamFor(resourceDescriptor, rootUrl),
                InferMediaTypeFrom(resourceDescriptor.Name)
            );
        }

        private Stream GetEmbeddedResourceStreamFor(EmbeddedAssetDescriptor resourceDescriptor, string rootUrl)
        {
            var stream = resourceDescriptor.Assembly.GetManifestResourceStream(resourceDescriptor.Name);
            if (stream == null)
                throw new AssetNotFound(String.Format("Embedded resource not found - {0}", resourceDescriptor.Name));

            if (resourceDescriptor.IsTemplate)
            {
                var templateParams = _templateParams
                    .Union(new[] { new KeyValuePair<string, string>("%(RootUrl)", rootUrl) })
                    .ToDictionary(entry => entry.Key, entry => entry.Value);

                return stream.FindAndReplace(templateParams);
            }

            return stream;
        }

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