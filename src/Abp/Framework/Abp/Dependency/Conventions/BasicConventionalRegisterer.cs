using System.Reflection;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Abp.Dependency.Conventions
{
    /// <summary>
    /// This class is used to register basic dependency implementations such as <see cref="ITransientDependency"/> and <see cref="ISingletonDependency"/>.
    /// </summary>
    internal class BasicConventionalRegisterer : IConventionalRegisterer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="windsorContainer"></param>
        /// <param name="assembly"></param>
        public void RegisterAssembly(IWindsorContainer windsorContainer, Assembly assembly)
        {
            //Transient
            windsorContainer.Register(
                Classes.FromAssembly(assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ITransientDependency>()
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                );

            //Singleton
            windsorContainer.Register(
                Classes.FromAssembly(assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ISingletonDependency>()
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleSingleton()
                );

            //Windsor Interceptors
            windsorContainer.Register(
                Classes.FromAssembly(assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<IInterceptor>()
                    .WithService.Self()
                    .LifestyleTransient()
                );
        }
    }
}