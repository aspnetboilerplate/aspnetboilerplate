using System.Collections.Generic;
using System.Web;
using Abp.Resources.Embedded;
using Microsoft.Owin.FileSystems;

namespace Abp.Owin
{
    public class AbpOwinEmbeddedResourceFileSystem : IFileSystem
    {
        public static HashSet<string> IgnoredFileExtensions { get; } = new HashSet<string>
        {
            "cshtml",
            "config"
        };

        private readonly IEmbeddedResourceManager _embeddedResourceManager;
        private readonly IFileSystem _physicalFileSystem;

        public AbpOwinEmbeddedResourceFileSystem(IEmbeddedResourceManager embeddedResourceManager)
        {
            _embeddedResourceManager = embeddedResourceManager;
            _physicalFileSystem = new PhysicalFileSystem(HttpContext.Current.Server.MapPath("~/"));
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            if (_physicalFileSystem.TryGetFileInfo(subpath, out fileInfo))
            {
                return true;
            }

            var resource = _embeddedResourceManager.GetResource(subpath);
            if (resource == null)
            {
                fileInfo = null;
                return false;
            }

            if (IsIgnoredFile(resource))
            {
                fileInfo = null;
                return false;
            }

            fileInfo = new AbpOwinEmbeddedResourceFileInfo(resource);
            return true;
        }

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            if (_physicalFileSystem.TryGetDirectoryContents(subpath, out contents))
            {
                return true;
            }

            //TODO: Implement!
            contents = null;
            return false;
        }

        private static bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != null && IgnoredFileExtensions.Contains(resource.FileExtension);
        }
    }
}