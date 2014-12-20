using System.Threading.Tasks;

namespace Abp.Domain.Uow
{

    internal class NullUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        public void Complete()
        {
            
        }

        public async Task CompleteAsync()
        {
           
        }

        public void Dispose()
        {

        }
    }
}