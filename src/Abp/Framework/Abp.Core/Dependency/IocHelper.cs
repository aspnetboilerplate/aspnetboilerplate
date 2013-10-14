namespace Abp.Dependency
{
    public static class IocHelper
    {
        public static T Resolve<T>()
        {
            return IocManager.IocContainer.Resolve<T>();
        }
    }
}