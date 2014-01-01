using Abp.Application.Authorization;
using Abp.Application.Services;
using Abp.Application.Services.Dto.Validation;
using Abp.Startup;
using Castle.Core;

namespace Abp.Application.Startup
{
    /// <summary>
    /// This class is used to register interceptors on the Application Layer.
    /// </summary>
    internal static class ApplicationLayerInterceptorRegisterer
    {
        public static void Initialize(IAbpInitializationContext initializationContext)
        {
            initializationContext.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;            
        }

        private static void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if (typeof(IApplicationService).IsAssignableFrom(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(ValidationInterceptor)));
                //TODO: If possible, intecept methods only those have AbpAuthorizeAttribute
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AuthorizationInterceptor))); 
            }
        }
    }
}