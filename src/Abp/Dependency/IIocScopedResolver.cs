using System;

namespace Abp.Dependency
{
    /// <summary>
    /// This interface is used to wrap a scope for batch resolvings in a single <c>using</c> statement.
    /// It inherits <see cref="IDisposable"/>, so resolved objects can be easily released by IocResolver.
    /// In <see cref="IDisposable.Dispose"/> method, <see cref="IIocResolver.Release"/> is called to dispose the object.
    /// </summary>
    public interface IIocScopedResolver : IDisposable
    {
        /// <summary>
        ///     Gets an object from scoped IOC resolver.
        ///     Returning object automatically released after usage.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The object instance</returns>
        T Resolve<T>();

        /// <summary>
        ///     Gets an object from scoped IOC resolver.
        ///     Returning object automatically released after usage.
        /// </summary>
        /// <typeparam name="T">Type of the object to cast</typeparam>
        /// <param name="type">Type of the object to resolve</param>
        /// <returns>The object instance</returns>
        T Resolve<T>(Type type);

        /// <summary>
        ///     Gets an object from scoped IOC resolver.
        ///     Returning object automatically released after usage.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        T Resolve<T>(object argumentsAsAnonymousType);

        /// <summary>
        ///     Gets an object from scoped IOC resolver.
        ///     Returning object automatically released after usage.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <returns>The object instance</returns>
        object Resolve(Type type);

        /// <summary>
        ///     Gets an object from scoped IOC resolver.
        ///     Returning object automatically released after usage.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        object Resolve(Type type, object argumentsAsAnonymousType);

        /// <summary>
        ///     Gets all implementations for given type from scoped IOC resolver.
        ///     Returning object automatically released after usage.
        /// </summary>
        /// <typeparam name="T">Type of the objects to resolve</typeparam>
        /// <returns>Object instances</returns>
        T[] ResolveAll<T>();

        /// <summary>
        ///     Gets all implementations for given type from scoped IOC resolver.
        ///     Returning object automatically released after usage.
        /// </summary>
        /// <typeparam name="T">Type of the objects to resolve</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>Object instances</returns>
        T[] ResolveAll<T>(object argumentsAsAnonymousType);

        ///// <summary>
        ///// Gets all implementations for given type from scoped IOC resolver.
        ///// Returning objects must be Released (see <see cref="Release"/>) after usage.
        ///// </summary> 
        ///// <param name="type">Type of the objects to resolve</param>
        ///// <returns>Object instances</returns>
        object[] ResolveAll(Type type);

        /// <summary>
        ///     Gets all implementations for given type from scoped IOC resolver.
        ///     Returning object automatically released after usage.
        /// </summary>
        /// <param name="type">Type of the objects to resolve</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>Object instances</returns>
        object[] ResolveAll(Type type, object argumentsAsAnonymousType);
    }
}
