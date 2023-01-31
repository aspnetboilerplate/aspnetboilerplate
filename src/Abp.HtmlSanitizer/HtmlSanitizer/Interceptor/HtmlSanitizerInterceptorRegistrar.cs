using Abp.Dependency;
using Castle.Core;
using Castle.MicroKernel;

namespace Abp.HtmlSanitizer.HtmlSanitizer.Interceptor
{
    public static class HtmlSanitizerInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.Register(typeof(AbpAsyncDeterminationInterceptor<HtmlSanitizerInterceptor>), DependencyLifeStyle.Transient);

            iocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        private static void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AbpAsyncDeterminationInterceptor<HtmlSanitizerInterceptor>)));
        }
    }
}
