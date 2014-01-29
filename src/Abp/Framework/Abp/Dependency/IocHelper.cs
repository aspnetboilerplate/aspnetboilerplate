using System;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is a helper to direcly use basic functionallity of dependency injection.
    /// Use <see cref="IocManager.IocContainer"/> to register dependencies.
    /// </summary>
    public static class IocHelper
    {
        #region Resolve

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

        #endregion

        #region ResolveAsDisposable

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>()
        {
            return new DisposableDependencyObjectWrapper<T>();
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="type">Type of the object to resolve. This type must be convertible <see cref="T"/>.</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(Type type)
        {
            return new DisposableDependencyObjectWrapper<T>(type);
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(object argumentsAsAnonymousType)
        {
            return new DisposableDependencyObjectWrapper<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="type">Type of the object to resolve. This type must be convertible <see cref="T"/>.</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(Type type, object argumentsAsAnonymousType)
        {
            return new DisposableDependencyObjectWrapper<T>(type, argumentsAsAnonymousType);
        }

        #endregion

        #region Release

        /// <summary>
        /// Releases a pre-resolved (see <see cref="Resolve{T}()"/>) object.
        /// </summary>
        /// <param name="obj">Object to be released</param>
        public static void Release(object obj)
        {
            IocManager.Instance.IocContainer.Release(obj);
        }

        #endregion
    }
}