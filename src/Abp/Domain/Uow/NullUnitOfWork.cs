using System;
using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Null implementation of unit of work.
    /// It's used if no component registered for <see cref="IUnitOfWork"/>.
    /// This ensures working ABP without a database.
    /// </summary>
    public sealed class NullUnitOfWork : UnitOfWorkBase
    {
        public override void SaveChanges()
        {
        }

        public async override Task SaveChangesAsync()
        {
        }

        [Obsolete("If you want to open the transaction, please use the BeginTransactional(UnitOfWorkOptions options)", false)]
        protected override void BeginUow()
        {
        }

        public override void BeginTransactional(UnitOfWorkOptions options)
        {
        }

        protected override void CompleteUow()
        {
        }

        protected async override Task CompleteUowAsync()
        {
        }

        protected override void DisposeUow()
        {
        }

        public NullUnitOfWork(IUnitOfWorkDefaultOptions defaultOptions)
            : base(defaultOptions)
        {
        }
    }
}