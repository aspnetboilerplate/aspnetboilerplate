using Abp.Dependency;
using Abp.Domain.Uow;
using MongoDB.Driver;

namespace Abp.MongoDb.Uow
{
    public class UnitOfWorkMongoDatabaseProvider : IMongoDatabaseProvider, ITransientDependency
    {
        public MongoDatabase Database { get { return ((MongoDbUnitOfWork)_currentUnitOfWork.Current).Database; } }

        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWork;

        public UnitOfWorkMongoDatabaseProvider(ICurrentUnitOfWorkProvider currentUnitOfWork)
        {
            _currentUnitOfWork = currentUnitOfWork;
        }
    }
}