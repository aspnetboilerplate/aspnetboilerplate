using System;
using System.IO;

namespace Abp.Web.Api.Swagger
{
    public interface IAssetProvider
    {
        Asset GetAsset(string rootUrl, string assetPath);
    }

    public class Asset
    {
        public Asset(Stream stream, string mediaType)
        {
            Stream = stream;
            MediaType = mediaType;
        }

        public Stream Stream { get; private set; }

        public string MediaType { get; private set; }
    }

    public class AssetNotFound : Exception
    {
        public AssetNotFound(string message)
            : base(message)
        {}
    }
}
