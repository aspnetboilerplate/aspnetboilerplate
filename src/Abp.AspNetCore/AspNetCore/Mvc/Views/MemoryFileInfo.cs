using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Abp.AspNetCore.Mvc.Views
{
    public class MemoryFileInfo : IFileInfo
    {
        public bool Exists => true;

        public long Length => _bytes.Length;

        public string PhysicalPath => null;

        public string Name { get; }

        public DateTimeOffset LastModified { get; }

        public bool IsDirectory => false;
        
        private readonly byte[] _bytes;

        public MemoryFileInfo(string name, byte[] bytes, DateTimeOffset lastModified)
        {
            _bytes = bytes;

            Name = name;
            LastModified = lastModified;
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(_bytes);
        }
    }
}