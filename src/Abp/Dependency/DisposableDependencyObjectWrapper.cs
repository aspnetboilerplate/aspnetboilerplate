using System;

namespace Abp.Dependency
{
    internal class DisposableDependencyObjectWrapper<T> : IDisposableDependencyObjectWrapper<T>
    {
        private readonly IIocResolver _iocResolver;

        public T Object { get; private set; }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            Object = iocResolver.Resolve<T>();
        }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, object argumentsAsAnonymousType)
        {
            _iocResolver = iocResolver;
            Object = iocResolver.Resolve<T>(argumentsAsAnonymousType);
        }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, Type type)
        {
            CheckType(type);
            _iocResolver = iocResolver;
            Object = (T)iocResolver.Resolve(type);
        }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, Type type, object argumentsAsAnonymousType)
        {
            CheckType(type);
            _iocResolver = iocResolver;
            Object = (T)iocResolver.Resolve(type, argumentsAsAnonymousType);
        }

        public void Dispose()
        {
            _iocResolver.Release(Object);
        }

        private static void CheckType(Type type)
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ArgumentException("Given type is not convertible to generic type definition!", "type");
            }
        }
    }

    internal class DisposableDependencyObjectWrapper : DisposableDependencyObjectWrapper<object>, IDisposableDependencyObjectWrapper
    {
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver)
            : base(iocResolver)
        {
        }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, object argumentsAsAnonymousType)
            : base(iocResolver, argumentsAsAnonymousType)
        {
        }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, Type type)
            : base(iocResolver, type)
        {
        }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, Type type, object argumentsAsAnonymousType)
            : base(iocResolver, type, argumentsAsAnonymousType)
        {
        }
    }
}