using System;
using System.Collections.Generic;
using Abp.AspNetCore.Configuration;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Abp.AspNetCore.Mvc.Views
{
    public class EmbeddedViewFileProvider : IFileProvider
    {
        private readonly Lazy<Dictionary<string, IFileInfo>> _files;

        private readonly IIocResolver _iocResolver;

        public EmbeddedViewFileProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            _files = new Lazy<Dictionary<string, IFileInfo>>(CreateFilesDictionary, true);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return _files.Value.GetOrDefault(subpath) ?? new NotFoundFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new NotFoundDirectoryContents();
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        private Dictionary<string, IFileInfo> CreateFilesDictionary()
        {
            var files = new Dictionary<string, IFileInfo>();

            var sources = _iocResolver.Resolve<IEmbeddedViewsConfiguration>().Sources;
            foreach (var source in sources)
            {
                source.AddFiles(files);
            }

            return files;
        }
    }
}