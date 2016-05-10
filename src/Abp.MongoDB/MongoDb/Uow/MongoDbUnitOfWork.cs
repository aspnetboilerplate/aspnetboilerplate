using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MongoDb.Configuration;
using MongoDB.Driver;

namespace Abp.MongoDb.Uow
{
    /// <summary>
    ///     Implements Unit of work for MongoDB.
    /// </summary>
    public class MongoDbUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        private readonly IAbpMongoDbModuleConfiguration _configuration;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public MongoDbUnitOfWork(IAbpMongoDbModuleConfiguration configuration,
            IConnectionStringResolver connectionStringResolver, IUnitOfWorkDefaultOptions defaultOptions)
            : base(connectionStringResolver, defaultOptions)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///     Gets a reference to MongoDB Database.
        /// </summary>
        public MongoDatabase Database { get; private set; }

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