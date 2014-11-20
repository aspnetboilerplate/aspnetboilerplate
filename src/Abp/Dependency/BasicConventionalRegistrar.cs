using Castle.DynamicProxy;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is used to register basic dependency implementations such as 
    /// <see cref="ITransientDependency"/>, <see cref="IScopedDependency"/> and <see cref="ISingletonDependency"/>.
    /// </summary>
    internal class BasicConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            // Transient
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ITransientDependency>()
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                );

            // Scoped (per web request or thread, if HttpContext is not available)
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<IScopedDependency>()
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleScoped<HybridPerWebRequestPerThreadScopeAccessor>()
                );

            // Singleton
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ISingletonDependency>()
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleSingleton()
                );

            // Windsor Interceptors
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<IInterceptor>()
                    .WithService.Self()
                    .LifestyleTransient()
                );
        }
    }
}