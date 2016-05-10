namespace Abp.Dependency
{
    internal class DisposableDependencyObjectWrapper : DisposableDependencyObjectWrapper<object>,
        IDisposableDependencyObjectWrapper
    {
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, object obj)
            : base(iocResolver, obj)
        {
        }
    }

    internal class DisposableDependencyObjectWrapper<T> : IDisposableDependencyObjectWrapper<T>
    {
        private readonly IIocResolver _iocResolver;

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, T obj)
        {
            _iocResolver = iocResolver;
            Object = obj;
        }

        public T Object { get; }

        public void Dispose()
        {
            _iocResolver.Release(Object);
        }
    }
}