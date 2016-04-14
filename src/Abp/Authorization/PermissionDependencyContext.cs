using Abp.Dependency;
using System;

namespace Abp.Authorization
{
    internal class PermissionDependencyContext : IPermissionDependencyContext, ITransientDependency
    {
        public Guid? UserId { get; set; }

        public IIocResolver IocResolver { get; private set; }

        public IPermissionChecker PermissionChecker { get; set; }

        public PermissionDependencyContext(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
            PermissionChecker = NullPermissionChecker.Instance;
        }
    }
}