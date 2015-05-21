using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.RavenDb.Configuration;
using Raven.Client;
using Raven.Client.Document;

namespace Abp.RavenDb.Uow
{
    /// <summary>
    /// Implements Unit of work for RavenDB.
    /// </summary>
    public class RavenDbUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        /// <summary>
        /// Gets a reference to RavenDB Database.
        /// </summary>
        public IDocumentSession Database { get; private set; }

        private readonly IAbpRavenDbModuleConfiguration _configuration;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RavenDbUnitOfWork(IAbpRavenDbModuleConfiguration configuration, IUnitOfWorkDefaultOptions defaultOptions)
            : base(defaultOptions)
        {
            _configuration = configuration;
        }

        protected override void BeginUow()
        {
            Database = RavenDocStore.Store(_configuration).OpenSession();
        }

        public override void SaveChanges()
        {
            Database.SaveChanges();
        }

        public override async Task SaveChangesAsync()
        {
            Database.SaveChanges();
        }

        protected override void CompleteUow()
        {

        }

        protected override async Task CompleteUowAsync()
        {

        }

        protected override void DisposeUow()
        {
            Database.Dispose();
        }
    }
}