using System;
using System.IO;

namespace Abp.WebApi.Swagger.Ui
{
    /// <summary>
    /// Asset Provider
    /// </summary>
    public interface IAssetProvider
    {
        /// <summary>
        /// Get embedded resource use Asset to wrap it.
        /// </summary>
        /// <param name="rootUrl">root url</param>
        /// <param name="assetPath">the path of asset</param>
        /// <returns></returns>
        Asset GetAsset(string rootUrl, string assetPath);
    }

    /// <summary>
    /// Asset Descriptor
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="mediaType">Media Type</param>
        public Asset(Stream stream, string mediaType)
        {
            Stream = stream;
            MediaType = mediaType;
        }

        /// <summary>
        /// stream
        /// </summary>
        public Stream Stream { get; private set; }
        /// <summary>
        /// media type
        /// </summary>
        public string MediaType { get; private set; }
    }

    /// <summary>
    /// Exception: Not found asset
    /// </summary>
    public class AssetNotFound : Exception
    {
        public AssetNotFound(string message)
            : base(message)
        {}
    }
}
