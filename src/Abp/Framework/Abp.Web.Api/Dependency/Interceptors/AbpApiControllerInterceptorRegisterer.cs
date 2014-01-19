using Abp.Startup;
using Abp.WebApi.Controllers;
using Castle.Core;

namespace Abp.WebApi.Dependency.Interceptors
{
    /// <summary>
    /// 
    /// </summary>
    public static class AbpApiControllerInterceptorRegisterer
    {
        public static void Initialize(IAbpInitializationContext initializationContext)
        {
            initializationContext.IocContainer.Kernel.ComponentRegistered += ComponentRegistered;

        }

        private static void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if (handler.ComponentModel.Implementation.IsSubclassOf(typeof(AbpApiController)))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AbpApiControllerInterceptor)));
            }
        }
    }
}