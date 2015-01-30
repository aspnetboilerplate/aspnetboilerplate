using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Null implementation of unit of work.
    /// It's used if no component registered for <see cref="IUnitOfWork"/>.
    /// </summary>
    public sealed class NullUnitOfWork : UnitOfWorkBase //TODO: Is that needed? Remove it..?
    {
        public override void SaveChanges()
        {
            
        }

        public async override Task SaveChangesAsync()
        {

        }

        protected override void BeginUow()
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
    }
}
