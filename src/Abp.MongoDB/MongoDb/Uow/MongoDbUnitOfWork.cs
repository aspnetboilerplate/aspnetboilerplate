using System.Threading.Tasks;
using Abp.Domain.Uow;
using MongoDB.Driver;

namespace Abp.MongoDb.Uow
{
    public class MongoDbUnitOfWork : UnitOfWorkBase
    {
        public MongoDatabase Database { get; set; }

        public override void Begin()
        {
            var client = new MongoClient("mongodb://localhost"); //TODO: Get from connection string???
            var server = client.GetServer();
            Database = server.GetDatabase("test"); //TODO: Get from connection string???
        }

        public override void SaveChanges()
        {

        }

        public override Task SaveChangesAsync()
        {
            throw new System.NotImplementedException();
        }

        public override void End()
        {

        }

        public override void Cancel()
        {

        }

        public override void Dispose()
        {

        }
    }
}