using Abp.Dependency;

namespace Abp.Authorization
{
    public class PermissionDependencyContext : IPermissionDependencyContext, ITransientDependency
    {
        public UserIdentifier User { get; set; }

        public IIocResolver IocResolver { get; }
        
        public IPermissionChecker PermissionChecker { get; set; }

        public PermissionDependencyContext(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
            PermissionChecker = NullPermissionChecker.Instance;
        }
    }
}