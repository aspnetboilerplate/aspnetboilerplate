using JetBrains.Annotations;

namespace Abp.BlobStoring
{
    public interface IBlobProviderSelector
    {
        [NotNull]
        IBlobProvider Get([NotNull] string containerName);
    }
}