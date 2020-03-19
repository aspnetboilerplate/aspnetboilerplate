using Abp.Domain.Uow;

namespace Abp.EntityFramework
{
    public class AbpEfDbContextInitializationContext
    {
        public IUnitOfWork UnitOfWork { get; }

        public AbpEfDbContextInitializationContext(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}
