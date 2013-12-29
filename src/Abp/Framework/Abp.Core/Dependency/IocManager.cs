using System;
using Castle.Windsor;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is used to create/dispose and get a reference to the dependency injection container.
    /// It implements Singleton pattern.
    /// </summary>
    internal class IocManager : IDisposable
    {
        /// <summary>
        /// Reference to the Castle Windsor Container.
        /// </summary>
        public WindsorContainer IocContainer { get;  private set; }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static IocManager Instance { get; private set; }

        static IocManager()
        {
            Instance = new IocManager();
        }

        private IocManager()
        {
            IocContainer = new WindsorContainer();
        }

        public void Dispose()
        {
            IocContainer.Dispose();
        }
    }
}