using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    internal class InnerSuppressUnitOfWorkCompleteHandle : InnerUnitOfWorkCompleteHandle
    {
        private readonly IUnitOfWork _parentUnitOfWork;

        public InnerSuppressUnitOfWorkCompleteHandle(IUnitOfWork parentUnitOfWork)
        {
            _parentUnitOfWork = parentUnitOfWork;
        }

        public override void Complete()
        {
            _parentUnitOfWork.SaveChanges();
            base.Complete();
        }

        public override async Task CompleteAsync()
        {
            await _parentUnitOfWork.SaveChangesAsync();
            await base.CompleteAsync();
        }
    }
}
