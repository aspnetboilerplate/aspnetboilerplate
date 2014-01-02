using System.Linq;
using Abp.Startup;
using Castle.Core;
using Castle.MicroKernel;

namespace Abp.Domain.Uow.NHibernate
{
    /// <summary>
    /// This class is used to register interceptor for needed classes for Unit Of Work mechanism.
    /// </summary>
    public static class NHibernateUnitOfWorkRegistrer
    {
        /// <summary>
        /// Initializes the registerer.
        /// </summary>
        /// <param name="initializationContext">Initialization context</param>
        public static void Initialize(IAbpInitializationContext initializationContext)
        {
            initializationContext.IocContainer.Kernel.ComponentRegistered += ComponentRegistered;
        }

        private static void ComponentRegistered(string key, IHandler handler)
        {
            if (UnitOfWorkHelper.IsRepositoryClass(handler.ComponentModel.Implementation))
            {
                //Intercept all methods of all repositories.
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(NhUnitOfWorkInterceptor)));
            }
            else if (handler.ComponentModel.Implementation.GetMethods().Any(UnitOfWorkHelper.HasUnitOfWorkAttribute))
            {
                //Intercept all methods of classes those have at least one method that has UnitOfWork attribute.
                //TODO: Intecept only UnitOfWork methods, not other methods!
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(NhUnitOfWorkInterceptor)));
            }
        }
    }
}
