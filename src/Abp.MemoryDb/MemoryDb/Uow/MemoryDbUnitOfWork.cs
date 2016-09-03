using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MemoryDb.Configuration;

namespace Abp.MemoryDb.Uow
{
    /// <summary>
    /// Implements Unit of work for MemoryDb.
    /// </summary>
    public class MemoryDbUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        /// <summary>
        /// Gets a reference to Memory Database.
        /// </summary>
        public MemoryDatabase Database { get; private set; }

        private readonly IAbpMemoryDbModuleConfiguration _configuration;
        private readonly MemoryDatabase _memoryDatabase;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MemoryDbUnitOfWork(
            IAbpMemoryDbModuleConfiguration configuration, 
            MemoryDatabase memoryDatabase, 
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkFilterExecuter filterExecuter,
            IUnitOfWorkDefaultOptions defaultOptions)
            : base(
                  connectionStringResolver, 
                  defaultOptions,
                  filterExecuter)
        {
            _configuration = configuration;
            _memoryDatabase = memoryDatabase;
        }

        protected override void BeginUow()
        {
            Database = _memoryDatabase;
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