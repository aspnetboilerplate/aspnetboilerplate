using Raven.Client;
using Raven.Client.Document;

namespace Abp.RavenDb
{
    /// <summary>
    /// Defines interface to obtain a <see cref="IDocumentSession"/> object.
    /// </summary>
    public interface IRavenDatabaseProvider
    {
        /// <summary>
        /// Gets the <see cref="IDocumentSession"/>.
        /// </summary>
        IDocumentSession Database { get; }
    }
}