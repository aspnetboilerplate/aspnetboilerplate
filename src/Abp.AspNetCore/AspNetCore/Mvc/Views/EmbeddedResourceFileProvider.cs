using System;
using Abp.Dependency;
using Abp.Resources.Embedded;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Abp.AspNetCore.Mvc.Views
{
    public class EmbeddedResourceFileProvider : IFileProvider
    {
        private IEmbeddedResourceManager EmbeddedResourceManager => _embeddedResourceManager.Value;
        private readonly Lazy<IEmbeddedResourceManager> _embeddedResourceManager;

        public EmbeddedResourceFileProvider(IIocResolver iocResolver)
        {
            _embeddedResourceManager = new Lazy<IEmbeddedResourceManager>(
                () => iocResolver.Resolve<IEmbeddedResourceManager>(),
                true
            );
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var resource = EmbeddedResourceManager.GetResource(subpath);

            if (resource == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            return new EmbeddedResourceItemFileInfo(resource);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            //TODO: Implement...?
            return new NotFoundDirectoryContents();
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}