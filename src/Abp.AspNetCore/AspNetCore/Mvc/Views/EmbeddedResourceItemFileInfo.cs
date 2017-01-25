using System;
using System.IO;
using Abp.Resources.Embedded;
using Microsoft.Extensions.FileProviders;

namespace Abp.AspNetCore.Mvc.Views
{
    public class EmbeddedResourceItemFileInfo : IFileInfo
    {
        public bool Exists => true;

        public long Length => _resourceItem.Content.Length;

        public string PhysicalPath => null;

        public string Name => _resourceItem.Name;

        public DateTimeOffset LastModified => _resourceItem.LastModifiedUtc;

        public bool IsDirectory => false;
        
        private readonly EmbeddedResourceItem _resourceItem;

        public EmbeddedResourceItemFileInfo(EmbeddedResourceItem resourceItem)
        {
            _resourceItem = resourceItem;
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(_resourceItem.Content);
        }
    }
}