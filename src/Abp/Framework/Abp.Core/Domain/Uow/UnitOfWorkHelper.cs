using System;
using System.Reflection;
using Abp.Domain.Repositories;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// A helper class to simplify unit of work process.
    /// </summary>
    public static class UnitOfWorkHelper
    {
        /// <summary>
        /// Determines if given method is a UnitOfWork method.
        /// </summary>
        /// <param name="methodInfo">Method information</param>
        /// <returns>True if should perform unit of work</returns>
        public static bool ShouldPerformUnitOfWork(MethodInfo methodInfo)
        {
            return HasUnitOfWorkAttribute(methodInfo) || IsRepositoryClass(methodInfo.DeclaringType);
        }

        /// <summary>
        /// Returns true if given type is a repository class (implements IRepository).
        /// </summary>
        /// <param name="type">Type to check</param>
        public static bool IsRepositoryClass(Type type)
        {
            return typeof(IRepository).IsAssignableFrom(type);
        }

        /// <summary>
        /// Returns true if given method has UnitOfWorkAttribute attribute.
        /// </summary>
        /// <param name="methodInfo">Method info to check</param>
        public static bool HasUnitOfWorkAttribute(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(UnitOfWorkAttribute), true);
        }
    }
}