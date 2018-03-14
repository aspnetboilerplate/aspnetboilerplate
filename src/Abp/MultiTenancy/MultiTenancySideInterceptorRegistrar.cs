using System;
using System.Linq;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Castle.Core;

namespace Abp.MultiTenancy
{
    internal static class MultiTenancySideInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                if (!iocManager.IsRegistered<IMultiTenancyConfig>())
                {
                    return;
                }

                var multiTenancyConfiguration = iocManager.Resolve<IMultiTenancyConfig>();

                if (ShouldIntercept(multiTenancyConfiguration, handler.ComponentModel.Implementation))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(MultiTenancySideAttribute)));
                }
            };
        }

        private static bool ShouldIntercept(IMultiTenancyConfig multiTenancyConfig, Type type)
        {
            if (!multiTenancyConfig.IsEnabled)
            {
                return false;
            }

            if (type.GetTypeInfo().IsDefined(typeof(MultiTenancySideAttribute), true))
            {
                return true;
            }

            if (type.GetMethods().Any(m => m.IsDefined(typeof(MultiTenancySideAttribute), true)))
            {
                return true;
            }

            return false;
        }
    }
}