using System.Reflection;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// Provides infrastructure to use any type of resources (files) embedded into assemblies.
    /// </summary>
    public interface IEmbeddedResourceManager
    {
        /// <summary>
        /// Makes possible to expose all files in a folder (and subfolders recursively).
        /// </summary>
        /// <param name="rootPath">
        /// Root folder path to be seen by clients.
        /// This is an arbitrary value with any deep.
        /// </param>
        /// <param name="assembly">The assembly contains resources.</param>
        /// <param name="resourceNamespace">Namespace in the <paramref name="assembly"/> that matches to the root path</param>
        void ExposeResources(string rootPath, Assembly assembly, string resourceNamespace);

        /// <summary>
        /// Used to get an embedded resource file.
        /// </summary>
        /// <param name="fullResourcePath">Full path of the resource</param>
        /// <returns>The resource</returns>
        EmbeddedResourceInfo GetResource(string fullResourcePath);
    }
}