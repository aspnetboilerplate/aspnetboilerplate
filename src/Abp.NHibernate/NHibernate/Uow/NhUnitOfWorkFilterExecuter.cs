using Abp.Domain.Uow;
using Abp.Extensions;

namespace Abp.NHibernate.Uow
{
    public class NhUnitOfWorkFilterExecuter : IUnitOfWorkFilterExecuter
    {
        public void ApplyDisableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            var session = unitOfWork.As<NhUnitOfWork>().Session;
            if (session.GetEnabledFilter(filterName) != null)
            {
                session.DisableFilter(filterName);
            }
        }

        public void ApplyEnableFilter(IUnitOfWork unitOfWork, string filterName)
        {
            var session = unitOfWork.As<NhUnitOfWork>().Session;
            if (session.GetEnabledFilter(filterName) == null)
            {
                session.EnableFilter(filterName);
            }
        }

        public void ApplyFilterParameterValue(IUnitOfWork unitOfWork, string filterName, string parameterName, object value)
        {
            var session = unitOfWork.As<NhUnitOfWork>().Session;
            if (session == null)
            {
                return;
            }

            var filter = session.GetEnabledFilter(filterName);
            if (filter != null)
            {
                filter.SetParameter(parameterName, value);
            }
        }
    }
}
