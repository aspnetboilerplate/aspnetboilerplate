using Castle.DynamicProxy;

namespace Abp.Domain.Uow
{
    public class AbpAsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor
        where TInterceptor : IAsyncInterceptor
    {
        public AbpAsyncDeterminationInterceptor(TInterceptor asyncInterceptor) : base(asyncInterceptor)
        {

        }
    }
}