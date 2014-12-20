using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    public class UnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly bool _startedByThisHandle;
        private bool _completed;

        public UnitOfWorkCompleteHandle(IUnitOfWork unitOfWork, bool startedByThisHandle)
        {
            _unitOfWork = unitOfWork;
            _startedByThisHandle = startedByThisHandle;
        }

        public void Complete()
        {
            SetAsCompleted();

            if (!_startedByThisHandle)
            {
                return;
            }

            _unitOfWork.Complete();
        }

        public async Task CompleteAsync()
        {
            SetAsCompleted();

            if (!_startedByThisHandle)
            {
                return;
            }

            await _unitOfWork.CompleteAsync();
        }

        public void Dispose()
        {
            if (!_startedByThisHandle)
            {
                return;
            }

            _unitOfWork.Dispose();
        }

        private void SetAsCompleted()
        {
            if (_completed)
            {
                throw new AbpException("Complete is called before!");
            }

            _completed = true;
        }
    }
}