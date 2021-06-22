using System;
using System.Collections.Generic;

namespace Abp.Runtime.Validation
{
    /// <summary>
    /// Used to get/set default options for a unit of work.
    /// </summary>
    public interface IAbpValidationDefaultOptions
    {
        /// <summary>
        /// A list of selectors to determine conventional classes for validation.
        /// </summary>
        List<Func<Type, bool>> ConventionalValidationSelectors { get; }
    }
}
