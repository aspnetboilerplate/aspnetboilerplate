using System;

namespace Abp.Dependency
{
    /// <summary>
    /// Extension methods to <see cref="IIocResolver"/> interface.
    /// </summary>
    public static class IocResolverExtensions
    {
        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocResolver">IIocResolver object</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver)
        {
            return new DisposableDependencyObjectWrapper<T>(iocResolver, iocResolver.Resolve<T>());
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocResolver">IIocResolver object</param>
        /// <param name="type">Type of the object to resolve. This type must be convertible <typeparamref name="T"/>.</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver, Type type)
        {
            return new DisposableDependencyObjectWrapper<T>(iocResolver, (T)iocResolver.Resolve(type));
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <param name="iocResolver">IIocResolver object</param>
        /// <param name="type">Type of the object to resolve. This type must be convertible to <see cref="IDisposable"/>.</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper ResolveAsDisposable(this IIocResolver iocResolver, Type type)
        {
            return new DisposableDependencyObjectWrapper(iocResolver, iocResolver.Resolve(type));
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocResolver">IIocResolver object</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver, object argumentsAsAnonymousType)
        {
            return new DisposableDependencyObjectWrapper<T>(iocResolver, iocResolver.Resolve<T>(argumentsAsAnonymousType));
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="iocResolver">IIocResolver object</param>
        /// <param name="type">Type of the object to resolve. This type must be convertible <typeparamref name="T"/>.</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver, Type type, object argumentsAsAnonymousType)
        {
            return new DisposableDependencyObjectWrapper<T>(iocResolver, (T)iocResolver.Resolve(type, argumentsAsAnonymousType)); 
        }

        /// <summary>
        /// Gets an <see cref="DisposableDependencyObjectWrapper{T}"/> object that wraps resolved object to be Disposable.
        /// </summary> 
        /// <param name="iocResolver">IIocResolver object</param>
        /// <param name="type">Type of the object to resolve. This type must be convertible to <see cref="IDisposable"/>.</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object wrapped by <see cref="DisposableDependencyObjectWrapper{T}"/></returns>
        public static IDisposableDependencyObjectWrapper ResolveAsDisposable(this IIocResolver iocResolver, Type type, object argumentsAsAnonymousType)
        {
            return new DisposableDependencyObjectWrapper(iocResolver, iocResolver.Resolve(type, argumentsAsAnonymousType));
        }

        /// <summary>
        /// Gets a <see cref="ScopedIocResolver"/> object that starts a scope to resolved objects to be Disposable.
        /// </summary>
        /// <param name="iocResolver"></param>
        /// <returns>The instance object wrapped by <see cref="ScopedIocResolver"/></returns>
        public static IScopedIocResolver CreateScope(this IIocResolver iocResolver)
        {
            return new ScopedIocResolver(iocResolver);
        }

        /// <summary>
        /// This method can be used to resolve and release an object automatically.
        /// You can use the object in <paramref name="action"/>.
        /// </summary> 
        /// <typeparam name="T">Type of the object to use</typeparam>
        /// <param name="iocResolver">IIocResolver object</param>
        /// <param name="action">An action that can use the resolved object</param>
        public static void Using<T>(this IIocResolver iocResolver, Action<T> action)
        {
            using (var wrapper = iocResolver.ResolveAsDisposable<T>())
            {
                action(wrapper.Object);
            }
        }

        /// <summary>
        /// This method can be used to resolve and release an object automatically.
        /// You can use the object in <paramref name="func"/> and return a value.
        /// </summary> 
        /// <typeparam name="TService">Type of the service to use</typeparam>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="iocResolver">IIocResolver object</param>
        /// <param name="func">A function that can use the resolved object</param>
        public static TReturn Using<TService, TReturn>(this IIocResolver iocResolver, Func<TService, TReturn> func)
        {
            using (var obj = iocResolver.ResolveAsDisposable<TService>())
            {
                return func(obj.Object);
            }
        }
        
        /// <summary>
        /// This method starts a scope to resolve and release all objects automatically.
        /// You can use the <c>scope</c> in <see cref="action"/>.
        /// </summary> 
        /// <param name="iocResolver">IIocResolver object</param>
        /// <param name="action">An action that can use the resolved object</param>
        public static void UsingScope(this IIocResolver iocResolver, Action<IScopedIocResolver> action)
        {
            using (var scope = iocResolver.CreateScope())
            {
                action(scope);
            }
        }
    }
}
