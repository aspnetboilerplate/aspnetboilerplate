using Abp.Dependency;
using Abp.FluentValidation.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using FluentValidation;

namespace Abp.FluentValidation
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpFluentValidationModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpFluentValidationConfiguration, AbpFluentValidationConfiguration>();
            IocManager.Register<AbpFluentValidationLanguageManager, AbpFluentValidationLanguageManager>();
            IocManager.Register<IValidatorFactory, AbpFluentValidationValidatorFactory>(DependencyLifeStyle.Transient);

            IocManager.AddConventionalRegistrar(new FluentValidationValidatorRegistrar());

            Configuration.Validation.Validators.Add<FluentValidationMethodParameterValidator>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpFluentValidationModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            ValidatorOptions.LanguageManager = IocManager.Resolve<AbpFluentValidationLanguageManager>();
        }
    }
}
