using System;
using Abp.Dependency;

namespace Abp.Authorization
{
    internal static class StaticPermissionChecker
    {
        private static readonly Lazy<IPermissionChecker> LazyInstance;

        static StaticPermissionChecker()
        {
            LazyInstance = new Lazy<IPermissionChecker>(
                () => IocManager.Instance.IsRegistered<IPermissionChecker>()
                    ? IocManager.Instance.Resolve<IPermissionChecker>()
                    : NullPermissionChecker.Instance
                );
        }

        public static IPermissionChecker Instance
        {
            get { return LazyInstance.Value; }
        }
    }
}