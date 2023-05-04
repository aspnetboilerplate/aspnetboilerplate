using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Abp.Dependency;
using Abp.Localization;

namespace Abp.BlobStoring.FileSystem
{
    public class FileSystemBlobNamingNormalizer : IBlobNamingNormalizer, ITransientDependency
    {

        public virtual string NormalizeContainerName(string containerName)
        {
            return Normalize(containerName);
        }

        public virtual string NormalizeBlobName(string blobName)
        {
            return Normalize(blobName);
        }

        protected virtual string Normalize(string fileName)
        {
            using (CultureInfoHelper.Use(CultureInfo.InvariantCulture))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // A filename cannot contain any of the following characters: \ / : * ? " < > |
                    // In order to support the directory included in the blob name, remove / and \
                    fileName = Regex.Replace(fileName, "[:\\*\\?\"<>\\|]", string.Empty);
                }

                return fileName;
            }
        }
    }
}
