using Abp.Dependency;
using Abp.Domain.Uow;
using MongoDB.Driver;

namespace Abp.MongoDb.Uow
{
    /// <summary>
    ///     Implements <see cref="IMongoDatabaseProvider" /> that gets database from active unit of work.
    /// </summary>
    public class UnitOfWorkMongoDatabaseProvider : IMongoDatabaseProvider, ITransientDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWork;

        public UnitOfWorkMongoDatabaseProvider(ICurrentUnitOfWorkProvider currentUnitOfWork)
        {
            _currentUnitOfWork = currentUnitOfWork;
        }

        public MongoDatabase Database
        {
            get { return ((MongoDbUnitOfWork) _currentUnitOfWork.Current).Database; }
        }
    }
}