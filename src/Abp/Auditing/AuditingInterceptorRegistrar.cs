using System;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Auditing
{
    internal static class AuditingInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                if (!iocManager.IsRegistered<IAuditingConfiguration>())
                {
                    return;
                }

                var auditingConfiguration = iocManager.Resolve<IAuditingConfiguration>();

                if (ShouldIntercept(auditingConfiguration, handler.ComponentModel.Implementation))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AuditingInterceptor)));
                }
            };
        }
        
        private static bool ShouldIntercept(IAuditingConfiguration auditingConfiguration, Type type)
        {
            if (auditingConfiguration.Selectors.Any(selector => selector.Predicate(type)))
            {
                return true;
            }

            if (type.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if (type.GetMethods().Any(m => m.IsDefined(typeof(AuditedAttribute), true)))
            {
                return true;
            }

            return false;
        }
    }
}