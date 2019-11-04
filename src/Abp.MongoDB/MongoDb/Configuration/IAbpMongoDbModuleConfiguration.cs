namespace Abp.MongoDb.Configuration
{
    public interface IAbpMongoDbModuleConfiguration
    {
        string ConnectionString { get; set; }

        string DatabaseName { get; set; }
    }
}
