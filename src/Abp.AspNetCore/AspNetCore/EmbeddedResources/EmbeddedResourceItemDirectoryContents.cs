using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Abp.Resources.Embedded;
using Microsoft.Extensions.FileProviders;

namespace Abp.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceItemDirectoryContents : IDirectoryContents
    {
        private readonly IEnumerable<IFileInfo> _entries;

        public EmbeddedResourceItemDirectoryContents(IEnumerable<IFileInfo> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }
            _entries = entries;
        }

        public bool Exists
        {
            get { return true; }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }
    }
}