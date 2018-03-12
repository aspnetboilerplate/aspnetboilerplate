using System;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Castle.Core;

namespace Abp.EntityHistory
{
    internal static class EntityHistoryInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                if (!iocManager.IsRegistered<IEntityHistoryConfiguration>())
                {
                    return;
                }

                var entityHistoryConfiguration = iocManager.Resolve<IEntityHistoryConfiguration>();

                if (ShouldIntercept(entityHistoryConfiguration, handler.ComponentModel.Implementation))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(EntityHistoryInterceptor)));
                }
            };
        }
        
        private static bool ShouldIntercept(IEntityHistoryConfiguration entityHistoryConfiguration, Type type)
        {
            if (type.GetTypeInfo().IsDefined(typeof(UseCaseAttribute), true))
            {
                return true;
            }

            if (type.GetMethods().Any(m => m.IsDefined(typeof(UseCaseAttribute), true)))
            {
                return true;
            }

            return false;
        }
    }
}
