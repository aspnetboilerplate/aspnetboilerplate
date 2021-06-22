using System;
using System.Linq;

namespace Abp.Runtime.Validation
{
    internal static class AbpValidationOptionsExtensions
    {
        public static bool IsConventionalValidationClass(this IAbpValidationDefaultOptions options, Type type)
        {
            return options.ConventionalValidationSelectors.Any(selector => selector(type));
        }
    }
}
