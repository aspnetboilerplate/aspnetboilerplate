using MongoDB.Driver;

namespace Abp.MongoDb
{
    public interface IMongoDatabaseProvider
    {
        MongoDatabase Database { get; }
    }
}