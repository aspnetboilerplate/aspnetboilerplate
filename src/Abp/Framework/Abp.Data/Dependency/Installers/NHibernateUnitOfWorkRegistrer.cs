using System.Linq;
using Abp.Data.Dependency.Interceptors;
using Abp.Data.Uow;
using Castle.Core;

namespace Abp.Data.Dependency.Installers
{
    /// <summary>
    /// This class is used to register interceptor for needed classes for Unit Of Work mechanism.
    /// </summary>
    public static class NHibernateUnitOfWorkRegistrer
    {
        /// <summary>
        /// This method is called on ComponentRegistered event of Castle Windsor.
        /// </summary>
        public static void ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
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
