using System;

namespace Abp.Dependency
{
    public class DisposableService<T> : IDisposable
    {
        public T Service { get; set; }

        public DisposableService(T service)
        {
            Service = service;
        }

        public void Dispose()
        {
            IocHelper.Release(Service);
        }
    }
}