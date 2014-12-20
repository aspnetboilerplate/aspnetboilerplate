using System;
using System.Web;
using Abp.Dependency;

namespace Abp.Domain.Uow.Web
{
    public class HttpContextUnitOfWorkScopeManager : IUnitOfWorkScopeManager, ISingletonDependency
    {
        private const string ContextKey = "Abp.UnitOfWork.Current";

        public IUnitOfWork Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return _unitOfWork;
                }

                return HttpContext.Current.Items[ContextKey] as IUnitOfWork;
            }

            set
            {
                if (HttpContext.Current == null)
                {
                    _unitOfWork = value;
                    return;
                }

                HttpContext.Current.Items[ContextKey] = value;
            }
        }

        [ThreadStatic]
        private static IUnitOfWork _unitOfWork;
    }
}
