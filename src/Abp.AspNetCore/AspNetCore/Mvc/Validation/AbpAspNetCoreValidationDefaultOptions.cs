using System;
using System.Collections.Generic;
using Abp.Runtime.Validation;

namespace Abp.AspNetCore.Mvc.Validation
{
    internal class AbpAspNetCoreValidationDefaultOptions : AbpValidationDefaultOptions
    {
        public AbpAspNetCoreValidationDefaultOptions()
        {
            ConventionalValidationSelectors = new List<Func<Type, bool>>();
        }
    }
}
