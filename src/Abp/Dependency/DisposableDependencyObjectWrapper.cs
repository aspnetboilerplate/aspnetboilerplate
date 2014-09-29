using System;

namespace Abp.Dependency
{
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