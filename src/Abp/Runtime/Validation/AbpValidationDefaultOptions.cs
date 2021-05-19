using System;
using System.Collections.Generic;
using Abp.Application.Services;

namespace Abp.Runtime.Validation
{
    public class AbpValidationDefaultOptions : IAbpValidationDefaultOptions
    {
        public List<Func<Type, bool>> ConventionalValidationSelectors { get; protected set; }

        public AbpValidationDefaultOptions()
        {
            ConventionalValidationSelectors = new List<Func<Type, bool>>
            {
                type => typeof(IApplicationService).IsAssignableFrom(type)
            };
        }
    }
}
