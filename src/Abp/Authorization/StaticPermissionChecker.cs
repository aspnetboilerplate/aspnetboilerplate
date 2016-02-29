using System;
using Adorable.Dependency;

namespace Adorable.Authorization
{
    internal static class StaticPermissionChecker
    {
        public static IPermissionChecker Instance { get { return LazyInstance.Value; } }
        private static readonly Lazy<IPermissionChecker> LazyInstance;

        static StaticPermissionChecker()
        {
            LazyInstance = new Lazy<IPermissionChecker>(
                () => IocManager.Instance.IsRegistered<IPermissionChecker>()
                    ? IocManager.Instance.Resolve<IPermissionChecker>()
                    : NullPermissionChecker.Instance
                );
        }
    }
}