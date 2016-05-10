using Abp.Dependency;

namespace Abp.Authorization
{
    internal class PermissionDependencyContext : IPermissionDependencyContext, ITransientDependency
    {
        public PermissionDependencyContext(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
            PermissionChecker = NullPermissionChecker.Instance;
        }

        public UserIdentifier User { get; set; }

        public IIocResolver IocResolver { get; }

        public IPermissionChecker PermissionChecker { get; set; }
    }
}