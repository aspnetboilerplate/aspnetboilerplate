using System;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is used to wrap a service that is resolved from IOC container.
    /// It implementes <see cref="IDisposable"/>, so resolved object can be easily released.
    /// This object is created by using <see cref="IocHelper.ResolveAsDisposable{T}()"/> method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisposableService<T> : IDisposable
    {
        /// <summary>
        /// The resolved object.
        /// </summary>
        public T Service { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="service">The resolved object</param>
        internal DisposableService(T service)
        {
            Service = service;
        }

        /// <summary>
        /// Releases the <see cref="Service"/> object.
        /// </summary>
        public void Dispose()
        {
            IocHelper.Release(Service);
        }
    }
}