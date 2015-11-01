using System;
using System.Linq;
using Abp.Dependency;
using Castle.Core;
using Castle.MicroKernel;

namespace Abp.Auditing
{
    internal static class AuditingInterceptorRegistrar
    {
        private static IAuditingConfiguration _auditingConfiguration;

        public static void Initialize(IIocManager iocManager)
        {
            _auditingConfiguration = iocManager.Resolve<IAuditingConfiguration>();

            if (!_auditingConfiguration.IsEnabled)
            {
                return;
            }

            iocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        private static void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            if (ShouldIntercept(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AuditingInterceptor)));
            }
        }

        private static bool ShouldIntercept(Type type)
        {
            if (_auditingConfiguration.Selectors.Any(selector => selector.Predicate(type)))
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