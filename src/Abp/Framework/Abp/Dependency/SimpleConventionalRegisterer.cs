using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Abp.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleConventionalRegisterer : IConventionalRegisterer
    {
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
        }
    }
}