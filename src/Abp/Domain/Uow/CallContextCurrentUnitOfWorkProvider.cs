using System.Runtime.Remoting.Messaging;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// CallContext implementation of <see cref="ICurrentUnitOfWorkProvider"/>. 
    /// This is default implementation.
    /// </summary>
    public class CallContextCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider, ISingletonDependency
    {
        private const string ContextKey = "Abp.UnitOfWork.Current";

        internal static IUnitOfWork StaticUow
        {
            get
            {
                var unitOfWork = CallContext.LogicalGetData(ContextKey) as IUnitOfWork;
                if (unitOfWork != null && unitOfWork.IsDisposed)
                {
                    CallContext.LogicalSetData(ContextKey, null);
                    return null;
                }

                return unitOfWork;
            }

            set
            {
                CallContext.LogicalSetData(ContextKey, value);
            }
        }

        /// <inheritdoc />
        [DoNotWire]
        public IUnitOfWork Current
        {
            get { return StaticUow; }
            set { StaticUow = value; }
        }
    }
}