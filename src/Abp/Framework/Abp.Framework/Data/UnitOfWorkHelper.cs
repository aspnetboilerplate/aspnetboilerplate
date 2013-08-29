using System;
using System.Reflection;
using Abp.Data.Repositories;

namespace Abp.Data
{
    /// <summary>
    /// A simple helper class to simplify unit of work process.
    /// </summary>
    public static class UnitOfWorkHelper
    {
        /// <summary>
        /// Returns true if given method is a member of a repository class.
        /// </summary>
        /// <param name="methodInfo">Method info to check</param>
        public static bool IsRepositoryMethod(MethodInfo methodInfo)
        {
            return IsRepositoryClass(methodInfo.DeclaringType);
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