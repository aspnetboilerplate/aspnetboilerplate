using System;
using Abp.Exceptions;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is used to wrap an object that is resolved from IOC container.
    /// It implementes <see cref="IDisposable"/>, so resolved object can be easily released.
    /// In <see cref="Dispose"/> method, <see cref="IocHelper.Release"/> is called to dispose the object.
    /// This object is created by using <see cref="IocHelper.ResolveAsDisposable{T}()"/> method. 
    /// </summary>
    /// <typeparam name="T">Type of the service</typeparam>
    public class DisposableDependencyObjectWrapper<T> : IDisposable
    {
        /// <summary>
        /// The resolved object.
        /// </summary>
        public T Object { get; private set; }

        internal DisposableDependencyObjectWrapper()
        {
            Object = IocHelper.Resolve<T>();
        }

        internal DisposableDependencyObjectWrapper(Type type)
        {
            if (!typeof (T).IsAssignableFrom(type))
            {
                throw new AbpException("Given type is compatible with generic type definition!");
            }

            Object = (T) IocHelper.Resolve(type);
        }

        internal DisposableDependencyObjectWrapper(object argumentsAsAnonymousType)
        {
            Object = IocHelper.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// Releases the <see cref="Object"/> object.
        /// </summary>
        public void Dispose()
        {
            IocHelper.Release(Object);
        }
    }
}