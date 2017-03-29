#if !NET46
using System.Threading;
using Abp.Dependency;
using Castle.Core;
using Castle.Core.Logging;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// CallContext implementation of <see cref="ICurrentUnitOfWorkProvider"/>. 
    /// This is the default implementation.
    /// </summary>
    public class AsyncLocalCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider, ITransientDependency
    {
        /// <inheritdoc />
        [DoNotWire]
        public IUnitOfWork Current
        {
            get { return GetCurrentUow(); }
            set { SetCurrentUow(value); }
        }

        public ILogger Logger { get; set; }

        private static readonly AsyncLocal<IUnitOfWork> AsyncLocalUow = new AsyncLocal<IUnitOfWork>();

        public AsyncLocalCurrentUnitOfWorkProvider()
        {
            Logger = NullLogger.Instance;
        }

        private static IUnitOfWork GetCurrentUow()
        {
            return AsyncLocalUow.Value;
        }

        private static void SetCurrentUow(IUnitOfWork value)
        {
            if (value == null)
            {
                //Set outer to current if possible
                if (AsyncLocalUow.Value != null)
                {
                    AsyncLocalUow.Value = AsyncLocalUow.Value.Outer;
                }

                return;
            }

            value.Outer = AsyncLocalUow.Value;
            AsyncLocalUow.Value = value;
        }
    }
}
#endif