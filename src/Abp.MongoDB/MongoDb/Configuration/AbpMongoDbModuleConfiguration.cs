namespace Abp.MongoDb.Configuration
{
    internal class AbpMongoDbModuleConfiguration : IAbpMongoDbModuleConfiguration
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}