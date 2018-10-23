using Abp.Dependency;
using Castle.MicroKernel.Registration;
using FluentValidation;

namespace Abp.FluentValidation
{
    public class FluentValidationValidatorRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .BasedOn(typeof(IValidator<>)).WithService.Base()
                    .LifestyleTransient()
            );
        }
    }
}
