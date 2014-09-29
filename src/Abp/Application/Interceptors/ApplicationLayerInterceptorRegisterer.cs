using Abp.Application.Authorization;
using Abp.Application.Services;
using Abp.Dependency;
using Abp.Runtime.Validation.Interception;
using Castle.Core;

namespace Abp.Application.Interceptors
{
    /// <summary>
    /// This class is used to register interceptors on the Application Layer.
    /// </summary>
    internal static class ApplicationLayerInterceptorRegisterer
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;            
        }

        private static void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if (typeof(IApplicationService).IsAssignableFrom(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(ValidationInterceptor)));

                //TODO: If possible, intercept methods only those have AbpAuthorizeAttribute
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AuthorizationInterceptor))); 
            }
        }
    }
}