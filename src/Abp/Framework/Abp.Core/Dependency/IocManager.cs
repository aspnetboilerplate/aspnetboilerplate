using Castle.Windsor;

namespace Abp.Dependency
{
    internal static class IocManager
    {
        public static WindsorContainer IocContainer { get; set; }

        public static void Initialize()
        {
            IocContainer = new WindsorContainer();
        }

        public static void Dispose()
        {
            if (IocContainer != null)
            {
                IocContainer.Dispose();                
            }
        }
    }
}