using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.MongoDb.Configuration;
using MongoDB.Driver;

namespace Abp.MongoDb.Uow
{
    public class MongoDbUnitOfWork : UnitOfWorkBase
    {
        public MongoDatabase Database { get; private set; }

        private readonly IAbpMongoDbModuleConfiguration _configuration;

        public MongoDbUnitOfWork(IAbpMongoDbModuleConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void BeginUow()
        {
            Database = new MongoClient(_configuration.ConnectionString)
                .GetServer()
                .GetDatabase(_configuration.DatatabaseName);
        }

        public override void SaveChanges()
        {

        }

        public override async Task SaveChangesAsync()
        {
            
        }

        protected override void CompleteUow()
        {

        }

        protected override async Task CompleteUowAsync()
        {

        }

        protected override void DisposeUow()
        {

        }
    }
}