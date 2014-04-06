using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Dependency.Conventions;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is used to create/dispose and get a reference to the dependency injection container.
    /// It implements Singleton pattern.
    /// </summary>
    public class IocManager : IDisposable
    {
        /// <summary>
        /// The Singleton instance
        /// </summary>
        public static IocManager Instance { get; private set; }

        /// <summary>
        /// Reference to the Castle Windsor Container.
        /// </summary>
        public IWindsorContainer IocContainer { get; private set; }

        private readonly List<IConventionalRegisterer> _conventionalRegisterers;

        static IocManager()
        {
            Instance = new IocManager();
        }

        private IocManager()
        {
            IocContainer = new WindsorContainer();
            _conventionalRegisterers = new List<IConventionalRegisterer>();
        }

        /// <summary>
        /// Adds a dependency registerer for conventional registration.
        /// </summary>
        /// <param name="registerer">dependency registerer</param>
        public void AddConventionalRegisterer(IConventionalRegisterer registerer)
        {
            _conventionalRegisterers.Add(registerer);
        }

        /// <summary>
        /// Registers types of given assembly by all conventional registerers. See <see cref="AddConventionalRegisterer"/> method.
        /// </summary>
        /// <param name="assembly">Assembly to register</param>
        public void RegisterAssemblyByConvention(Assembly assembly)
        {
            RegisterAssemblyByConvention(assembly, new ConventionalRegistrationConfig());
        }

        /// <summary>
        /// Registers types of given assembly by all conventional registerers. See <see cref="AddConventionalRegisterer"/> method.
        /// </summary>
        /// <param name="assembly">Assembly to register</param>
        /// <param name="config">Additional configuration</param>
        public void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config)
        {
            var context = new ConventionalRegistrationContext(assembly, IocContainer, config);

            foreach (var registerer in _conventionalRegisterers)
            {
                registerer.RegisterAssembly(context);
            }

            if (config.InstallInstallers)
            {
                IocContainer.Install(FromAssembly.Instance(assembly));                
            }
        }

        public void Dispose()
        {
            IocContainer.Dispose();
        }
    }
}