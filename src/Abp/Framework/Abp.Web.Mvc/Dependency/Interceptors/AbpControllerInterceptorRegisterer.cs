using Abp.Startup;
using Abp.Web.Mvc.Controllers;
using Castle.Core;

namespace Abp.Web.Mvc.Dependency.Interceptors
{
    /// <summary>
    /// 
    /// </summary>
    public static class AbpControllerInterceptorRegisterer
    {
        public static void Initialize(IAbpInitializationContext initializationContext)
        {
            initializationContext.IocContainer.Kernel.ComponentRegistered += ComponentRegistered;
        }

        private static void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if (handler.ComponentModel.Implementation.IsSubclassOf(typeof(AbpController)))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AbpControllerInterceptor)));
            }
        }
    }
}