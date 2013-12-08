using Castle.Windsor;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is used to create/dispose and get a reference to the dependency injection container.
    /// </summary>
    internal static class IocManager
    {
        /// <summary>
        /// Reference to the Castle Windsor Container.
        /// </summary>
        public static WindsorContainer IocContainer { get;  private set; }

        /// <summary>
        /// Creates <see cref="IocContainer"/> object.
        /// </summary>
        public static void Initialize()
        {
            if (IocContainer != null)
            {
                return;
            }

            IocContainer = new WindsorContainer();
        }

        /// <summary>
        /// Disposes <see cref="IocContainer"/> object.
        /// </summary>
        public static void Dispose()
        {
            if (IocContainer == null)
            {
                return;
            }

            IocContainer.Dispose();
            IocContainer = null;
        }
    }
}