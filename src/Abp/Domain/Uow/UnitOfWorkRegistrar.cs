using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Castle.Core;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This class is used to register interceptor for needed classes for Unit Of Work mechanism.
    /// </summary>
    internal static class UnitOfWorkRegistrar
    {
        /// <summary>
        /// Initializes the registerer.
        /// </summary>
        /// <param name="iocManager">IOC manager</param>
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                var implementationType = handler.ComponentModel.Implementation.GetTypeInfo();

                if (ShouldIntercept(iocManager, implementationType))
                {
                    handler.ComponentModel.Interceptors.Add(
                        new InterceptorReference(typeof(AbpAsyncDeterminationInterceptor<UnitOfWorkInterceptor>))
                    );
                }
            };
        }

        private static bool ShouldIntercept(IIocManager iocManager, TypeInfo implementationType)
        {
            if (IsUnitOfWorkType(implementationType) || AnyMethodHasUnitOfWork(implementationType))
            {
                return true;
            }
            
            if (!iocManager.IsRegistered<IUnitOfWorkDefaultOptions>())
            {
                return false;
            }

            var uowOptions = iocManager.Resolve<IUnitOfWorkDefaultOptions>();

            return uowOptions.IsConventionalUowClass(implementationType.AsType());
        }

        private static bool IsUnitOfWorkType(TypeInfo implementationType)
        {
            return UnitOfWorkHelper.HasUnitOfWorkAttribute(implementationType);
        }

        private static bool AnyMethodHasUnitOfWork(TypeInfo implementationType)
        {
            return implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(UnitOfWorkHelper.HasUnitOfWorkAttribute);
        }
    }
}
