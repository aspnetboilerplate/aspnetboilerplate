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
                if (!iocManager.IsRegistered<IUnitOfWorkDefaultOptions>())
                {
                    return;
                }

                var uowOptions = iocManager.Resolve<IUnitOfWorkDefaultOptions>();

                if (uowOptions.IsConventionalUowClass(handler.ComponentModel.Implementation))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
                }
                else if (UnitOfWorkHelper.HasUnitOfWorkAttribute(handler.ComponentModel.Implementation.GetTypeInfo()) ||
                        handler.ComponentModel
                               .Implementation
                               .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                               .Any(UnitOfWorkHelper.HasUnitOfWorkAttribute))
                {
                    //Intercept all methods of classes those have at least one method that has UnitOfWork attribute or class marked as UnitOfWork.
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
                }
            };
        }
    }
}