namespace Adorable.MongoDb.Configuration
{
    public interface IAbpMongoDbModuleConfiguration
    {
        string ConnectionString { get; set; }

        string DatatabaseName { get; set; }
    }
}
