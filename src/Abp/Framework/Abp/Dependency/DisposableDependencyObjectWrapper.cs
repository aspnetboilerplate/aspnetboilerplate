using System;

namespace Abp.Dependency
{
    internal class DisposableDependencyObjectWrapper<T> : IDisposableDependencyObjectWrapper<T>
    {
        private readonly IocManager _iocManager;

        public T Object { get; private set; }

        public DisposableDependencyObjectWrapper(IocManager iocManager)
        {
            _iocManager = iocManager;
            Object = iocManager.Resolve<T>();
        }

        public DisposableDependencyObjectWrapper(IocManager iocManager, object argumentsAsAnonymousType)
        {
            Object = iocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        public DisposableDependencyObjectWrapper(IocManager iocManager, Type type)
        {
            CheckType(type);
            Object = (T)iocManager.Resolve(type);
        }

        public DisposableDependencyObjectWrapper(IocManager iocManager, Type type, object argumentsAsAnonymousType)
        {
            CheckType(type);
            Object = (T)iocManager.Resolve(type, argumentsAsAnonymousType);
        }

        public void Dispose()
        {
            _iocManager.Release(Object);
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
        public DisposableDependencyObjectWrapper(IocManager iocManager)
            : base(iocManager)
        {
        }

        public DisposableDependencyObjectWrapper(IocManager iocManager, object argumentsAsAnonymousType)
            : base(iocManager, argumentsAsAnonymousType)
        {
        }

        public DisposableDependencyObjectWrapper(IocManager iocManager, Type type)
            : base(iocManager, type)
        {
        }

        public DisposableDependencyObjectWrapper(IocManager iocManager, Type type, object argumentsAsAnonymousType)
            : base(iocManager, type, argumentsAsAnonymousType)
        {
        }
    }
}