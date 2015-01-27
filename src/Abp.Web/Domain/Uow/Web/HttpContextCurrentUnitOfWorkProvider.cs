using System.Runtime.Remoting.Messaging;
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
                    var unitOfWork = CallContext.LogicalGetData(ContextKey) as IUnitOfWork;

                    if (unitOfWork != null && unitOfWork.IsDisposed)
                    {
                        unitOfWork = null;
                    }

                    return unitOfWork; 
                }

                return HttpContext.Current.Items[ContextKey] as IUnitOfWork;
            }

            set
            {
                if (HttpContext.Current == null)
                {
                    CallContext.LogicalSetData(ContextKey, value);
                    return;
                }

                HttpContext.Current.Items[ContextKey] = value;
            }
        }
    }
}
