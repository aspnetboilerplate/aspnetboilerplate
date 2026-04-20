using System.Globalization;
using System.Text.RegularExpressions;
using Abp.Localization;
using Abp.Dependency;

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
                // A filename cannot contain any of the following characters: \ / : * ? " < > |
                // In order to support the directory included in the blob name, remove invalid filename characters.
                fileName = Regex.Replace(fileName, "[:\\*\\?\"<>\\|]", string.Empty);

                return fileName;
            }
        }
    }
}
