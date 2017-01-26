using JetBrains.Annotations;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// Provides infrastructure to use any type of resources (files) embedded into assemblies.
    /// </summary>
    public interface IEmbeddedResourceManager
    {
        /// <summary>
        /// Used to get an embedded resource file.
        /// Can return null if resource is not found!
        /// </summary>
        /// <param name="fullResourcePath">Full path of the resource</param>
        /// <returns>The resource</returns>
        [CanBeNull]
        EmbeddedResourceItem GetResource([NotNull] string fullResourcePath);
    }
}