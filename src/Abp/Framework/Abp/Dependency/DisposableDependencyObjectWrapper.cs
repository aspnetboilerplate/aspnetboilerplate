using System;

namespace Abp.Dependency
{
    internal class DisposableDependencyObjectWrapper<T> : IDisposableDependencyObjectWrapper<T>
    {
        public T Object { get; private set; }

        public DisposableDependencyObjectWrapper()
        {
            Object = IocHelper.Resolve<T>();
        }

        public DisposableDependencyObjectWrapper(object argumentsAsAnonymousType)
        {
            Object = IocHelper.Resolve<T>(argumentsAsAnonymousType);
        }

        public DisposableDependencyObjectWrapper(Type type)
        {
            CheckType(type);
            Object = (T)IocHelper.Resolve(type);
        }

        public DisposableDependencyObjectWrapper(Type type, object argumentsAsAnonymousType)
        {
            CheckType(type);
            Object = (T)IocHelper.Resolve(type, argumentsAsAnonymousType);
        }

        public void Dispose()
        {
            IocHelper.Release(Object);
        }

        private static void CheckType(Type type)
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ArgumentException("Given type is not convertible to generic type definition!", "type");
            }
        }
    }
}