using System;
using System.Linq;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Auditing
{
    internal static class AuditingInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            var auditingConfiguration = iocManager.Resolve<IAuditingConfiguration>();
            if (!auditingConfiguration.IsEnabled)
            {
                return;
            }

            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
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

            if (type.IsDefined(typeof(AuditedAttribute), true)) //TODO: true or false?
            {
                return true;
            }

            if (type.GetMethods().Any(m => m.IsDefined(typeof(AuditedAttribute), true))) //TODO: true or false?
            {
                return true;
            }

            return false;
        }
    }
}