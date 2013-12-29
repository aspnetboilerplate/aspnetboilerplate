using System;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is a helper to direcly use basic functionallity of dependency injection.
    /// </summary>
    public static class IocHelper
    {
        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IocHelper.Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The instance object</returns>
        public static T Resolve<T>()
        {
            return IocManager.Instance.IocContainer.Resolve<T>();
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IocHelper.Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object</returns>
        public static T Resolve<T>(object argumentsAsAnonymousType) //TODO: Test!
        {
            return IocManager.Instance.IocContainer.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IocHelper.Release"/>) after usage.
        /// </summary> 
        /// <param name="type">Type of the object to get</param>
        /// <returns>The instance object</returns>
        public static object Resolve(Type type)
        {
            return IocManager.Instance.IocContainer.Resolve(type);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IocHelper.Release"/>) after usage.
        /// </summary> 
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object</returns>
        public static object Resolve(Type type, object argumentsAsAnonymousType)
        {
            return IocManager.Instance.IocContainer.Resolve(type, argumentsAsAnonymousType);
        }

        /// <summary>
        /// Gets an <see cref="DisposableService{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The instance object wrapped by <see cref="DisposableService{T}"/></returns>
        public static DisposableService<T> ResolveAsDisposable<T>()
        {
            return new DisposableService<T>(Resolve<T>());
        }

        /// <summary>
        /// Gets an <see cref="DisposableService{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableService{T}"/></returns>
        public static DisposableService<T> ResolveAsDisposable<T>(object argumentsAsAnonymousType)
        {
            return new DisposableService<T>(Resolve<T>(argumentsAsAnonymousType));
        }

        /// <summary>
        /// Releases a pre-resolved (see <see cref="Resolve{T}()"/>) object.
        /// </summary>
        /// <param name="obj">Object to be released</param>
        public static void Release(object obj)
        {
            IocManager.Instance.IocContainer.Release(obj);
        }
    }
}