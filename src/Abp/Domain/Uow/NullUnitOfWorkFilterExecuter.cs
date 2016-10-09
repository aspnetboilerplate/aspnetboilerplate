namespace Abp.Domain.Uow
{
    public class NullUnitOfWorkFilterExecuter : IUnitOfWorkFilterExecuter
    {
        public void ApplyDisableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            
        }

        public void ApplyEnableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            
        }

        public void ApplyFilterParameterValue(IUnitOfWork unitOfWork, string filterName, string parameterName, object value)
        {
            
        }
    }
}