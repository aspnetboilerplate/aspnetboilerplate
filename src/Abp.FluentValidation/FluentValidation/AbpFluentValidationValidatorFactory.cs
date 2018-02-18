using System;
using Abp.Dependency;
using FluentValidation;

namespace Abp.FluentValidation
{
    public class AbpFluentValidationValidatorFactory : ValidatorFactoryBase
    {
        private readonly IIocResolver _iocResolver;

        public AbpFluentValidationValidatorFactory(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            if (_iocResolver.IsRegistered(validatorType))
            {
                return _iocResolver.Resolve(validatorType) as IValidator;
            }

            return null;
        }
    }
}
