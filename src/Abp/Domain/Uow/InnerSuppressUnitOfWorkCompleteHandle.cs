using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    internal class InnerSuppressUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        private readonly IUnitOfWork _parentUnitOfWork;

        public InnerSuppressUnitOfWorkCompleteHandle(IUnitOfWork parentUnitOfWork)
        {
            _parentUnitOfWork = parentUnitOfWork;
        }

        public void Complete()
        {
            _parentUnitOfWork.SaveChanges();
        }

        public async Task CompleteAsync()
        {
            await _parentUnitOfWork.SaveChangesAsync();
        }

        public void Dispose()
        {

        }
    }
}
