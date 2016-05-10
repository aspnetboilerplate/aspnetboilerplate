using Abp.Dependency;
using Abp.Domain.Uow;

namespace Abp.MemoryDb.Uow
{
    /// <summary>
    ///     Implements <see cref="IMemoryDatabaseProvider" /> that gets database from active unit of work.
    /// </summary>
    public class UnitOfWorkMemoryDatabaseProvider : IMemoryDatabaseProvider, ITransientDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWork;

        public UnitOfWorkMemoryDatabaseProvider(ICurrentUnitOfWorkProvider currentUnitOfWork)
        {
            _currentUnitOfWork = currentUnitOfWork;
        }

        public MemoryDatabase Database
        {
            get { return ((MemoryDbUnitOfWork) _currentUnitOfWork.Current).Database; }
        }
    }
}