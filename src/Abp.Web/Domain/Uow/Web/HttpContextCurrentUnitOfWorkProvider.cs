using System;
using System.Web;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Domain.Uow.Web
{
    /// <summary>
    /// Implements <see cref="ICurrentUnitOfWorkProvider"/> using <see cref="HttpContext.Current"/>.
    /// Fallbacks to ThreadStatic if <see cref="HttpContext.Current"/> is invalid.
    /// </summary>
    public class HttpContextCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider, ISingletonDependency
    {
        private const string ContextKey = "Abp.UnitOfWork.Current";

        [DoNotWire]
        public IUnitOfWork Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return _unitOfWork; //Fallback
                }

                return HttpContext.Current.Items[ContextKey] as IUnitOfWork;
            }

            set
            {
                if (HttpContext.Current == null)
                {
                    _unitOfWork = value;  //Fallback
                    return;
                }

                HttpContext.Current.Items[ContextKey] = value;
            }
        }

        [ThreadStatic]
        private static IUnitOfWork _unitOfWork;
    }
}
