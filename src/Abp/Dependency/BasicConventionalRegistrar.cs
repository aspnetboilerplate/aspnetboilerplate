using System.Reflection;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;

namespace Abp.Dependency
{
    /// <summary>
    /// This class is used to register basic dependency implementations such as <see cref="ITransientDependency"/> and <see cref="ISingletonDependency"/>.
    /// </summary>
    public class BasicConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            //Transient
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ITransientDependency>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                );

            //Singleton
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ISingletonDependency>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleSingleton()
                );

            //Windsor Interceptors
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<IInterceptor>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                    .WithService.Self()
                    .LifestyleTransient()
                );
        }
    }
}