using MongoDB.Driver;

namespace Abp.MongoDb
{
    /// <summary>
    /// Defines interface to obtain a <see cref="MongoDatabase"/> object.
    /// </summary>
    public interface IMongoDatabaseProvider
    {
        /// <summary>
        /// Gets the <see cref="MongoDatabase"/>.
        /// </summary>
        MongoDatabase Database { get; }
    }
}