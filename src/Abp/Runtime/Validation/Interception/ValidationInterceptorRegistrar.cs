using System.Reflection;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Runtime.Validation.Interception
{
    internal static class ValidationInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                var implementationType = handler.ComponentModel.Implementation.GetTypeInfo();
            
                if (!iocManager.IsRegistered<IAbpValidationDefaultOptions>())
                {
                    return;
                }
                
                var validationOptions = iocManager.Resolve<IAbpValidationDefaultOptions>();

                if (validationOptions.IsConventionalValidationClass(implementationType.AsType()))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AbpAsyncDeterminationInterceptor<ValidationInterceptor>)));
                }
            };
        }
    }
}
