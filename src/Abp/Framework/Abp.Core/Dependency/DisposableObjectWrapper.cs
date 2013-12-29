using System;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is used to wrap an object that is resolved from IOC container.
    /// It implementes <see cref="IDisposable"/>, so resolved object can be easily released.
    /// This object is created by using <see cref="IocHelper.ResolveAsDisposable{T}()"/> method.
    /// </summary>
    /// <typeparam name="T">Type of the service</typeparam>
    public class DisposableObjectWrapper<T> : IDisposable
    {
        /// <summary>
        /// The resolved object.
        /// </summary>
        public T Object { get; private set; }

        internal DisposableObjectWrapper(T obj)
        {
            Object = obj;
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