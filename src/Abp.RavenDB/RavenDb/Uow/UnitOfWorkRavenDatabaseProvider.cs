using Abp.Dependency;
using Abp.Domain.Uow;
using Raven.Client;
using Raven.Client.Document;

namespace Abp.RavenDb.Uow
{
    /// <summary>
    /// Implements <see cref="IRavenDatabaseProvider"/> that gets database from active unit of work.
    /// </summary>
    public class UnitOfWorkRavenDatabaseProvider : IRavenDatabaseProvider, ITransientDependency
    {
        public IDocumentSession Database { get { return ((RavenDbUnitOfWork)_currentUnitOfWork.Current).Database; } }

        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWork;

        public UnitOfWorkRavenDatabaseProvider(ICurrentUnitOfWorkProvider currentUnitOfWork)
        {
            _currentUnitOfWork = currentUnitOfWork;
        }
    }
}