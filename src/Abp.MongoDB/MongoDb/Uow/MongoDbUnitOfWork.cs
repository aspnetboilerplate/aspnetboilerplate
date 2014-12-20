using System.Threading.Tasks;
using Abp.Domain.Uow;
using MongoDB.Driver;

namespace Abp.MongoDb.Uow
{
    public class MongoDbUnitOfWork : UnitOfWorkBase
    {
        public MongoDatabase Database { get; set; }

        protected override void StartUow()
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

        protected override void CompleteUow()
        {

        }

        protected override Task CompleteUowAsync()
        {
            throw new System.NotImplementedException();
        }

        protected override void DisposeUow()
        {

        }
    }
}