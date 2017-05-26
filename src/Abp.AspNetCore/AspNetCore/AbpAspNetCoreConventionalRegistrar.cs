using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Abp.AspNetCore
{
    public class AbpAspNetCoreConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            //ViewComponents
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .BasedOn<ViewComponent>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                    .LifestyleTransient()
            );
        }
    }
}
