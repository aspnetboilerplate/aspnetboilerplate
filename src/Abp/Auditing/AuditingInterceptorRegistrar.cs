using System;
using System.Linq;
using Abp.Dependency;
using Castle.Core;
using Castle.MicroKernel;

namespace Abp.Auditing
{
    internal class AuditingInterceptorRegistrar
    {
        private readonly IAuditingConfiguration _auditingConfiguration;
        private readonly IIocManager _iocManager;

        public AuditingInterceptorRegistrar(IAuditingConfiguration auditingConfiguration, IIocManager iocManager)
        {
            _auditingConfiguration = auditingConfiguration;
            _iocManager = iocManager;
        }

        public void Initialize()
        {
            if (!_auditingConfiguration.IsEnabled)
            {
                return;
            }

            _iocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        private void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            if (ShouldIntercept(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AuditingInterceptor)));
            }
        }

        private bool ShouldIntercept(Type type)
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