namespace Abp.Dependency
{
    public static class IocHelper
    {
        public static T Resolve<T>()
        {
            return IocManager.IocContainer.Resolve<T>();
        }

        public static void Release(object obj)
        {
            IocManager.IocContainer.Release(obj);
        }

        public static DisposableService<T> ResolveDisposableService<T>()
        {
            return new DisposableService<T>(Resolve<T>());
        }
    }
}