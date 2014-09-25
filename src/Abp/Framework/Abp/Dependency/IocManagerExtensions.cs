using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Dependency
{
    public static class IocManagerExtensions
    {
        #region Resolve

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IocHelper.Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocManager">IocManager object</param>
        /// <returns>The instance object</returns>
        public static T Resolve<T>(this IocManager iocManager)
        {
            return iocManager.IocContainer.Resolve<T>();
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IocHelper.Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocManager">IocManager object</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object</returns>
        public static T Resolve<T>(this IocManager iocManager, object argumentsAsAnonymousType) //TODO: Test!
        {
            return iocManager.IocContainer.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IocHelper.Release"/>) after usage.
        /// </summary> 
        /// <param name="type">Type of the object to get</param>
        /// <param name="iocManager">IocManager object</param>
        /// <returns>The instance object</returns>
        public static object Resolve(this IocManager iocManager, Type type)
        {
            return iocManager.IocContainer.Resolve(type);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IocHelper.Release"/>) after usage.
        /// </summary> 
        /// <param name="type">Type of the object to get</param>
        /// <param name="iocManager">IocManager object</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object</returns>
        public static object Resolve(this IocManager iocManager, Type type, object argumentsAsAnonymousType)
        {
            return iocManager.IocContainer.Resolve(type, argumentsAsAnonymousType);
        }

        #endregion

        #region ResolveAsDisposable

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocManager">IocManager object</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IocManager iocManager)
        {
            return new DisposableDependencyObjectWrapper<T>(iocManager);
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocManager">IocManager object</param>
        /// <param name="type">Type of the object to resolve. This type must be convertible <see cref="T"/>.</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IocManager iocManager, Type type)
        {
            return new DisposableDependencyObjectWrapper<T>(iocManager, type);
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <param name="iocManager">IocManager object</param>
        /// <param name="type">Type of the object to resolve. This type must be convertible <see cref="T"/>.</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper ResolveAsDisposable(this IocManager iocManager, Type type)
        {
            return new DisposableDependencyObjectWrapper(iocManager, type);
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocManager">IocManager object</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IocManager iocManager, object argumentsAsAnonymousType)
        {
            return new DisposableDependencyObjectWrapper<T>(iocManager, argumentsAsAnonymousType);
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocManager">IocManager object</param>
        /// <param name="type">Type of the object to resolve. This type must be convertible <see cref="T"/>.</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IocManager iocManager, Type type, object argumentsAsAnonymousType)
        {
            return new DisposableDependencyObjectWrapper<T>(iocManager, type, argumentsAsAnonymousType);
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <param name="iocManager">IocManager object</param>
        /// <param name="type">Type of the object to resolve. This type must be convertible <see cref="T"/>.</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper ResolveAsDisposable(this IocManager iocManager, Type type, object argumentsAsAnonymousType)
        {
            return new DisposableDependencyObjectWrapper(iocManager, type, argumentsAsAnonymousType);
        }

        #endregion

        #region Release

        /// <summary>
        /// Releases a pre-resolved object. See <see cref="Resolve{T}(IocManager)"/> method.
        /// </summary>
        /// <param name="iocManager">IocManager object</param>
        /// <param name="obj">Object to be released</param>
        public static void Release(this IocManager iocManager, object obj)
        {
            iocManager.IocContainer.Release(obj);
        }

        #endregion
    }
}
