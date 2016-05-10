using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    /// <summary>
    ///     Null implementation of unit of work.
    ///     It's used if no component registered for <see cref="IUnitOfWork" />.
    ///     This ensures working ABP without a database.
    /// </summary>
    public sealed class NullUnitOfWork : UnitOfWorkBase
    {
        public NullUnitOfWork(IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkDefaultOptions defaultOptions)
            : base(connectionStringResolver, defaultOptions)
        {
        }

        public override void SaveChanges()
        {
        }

        public override async Task SaveChangesAsync()
        {
        }

        protected override void BeginUow()
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