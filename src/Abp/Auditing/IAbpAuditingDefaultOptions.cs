using System;
using System.Collections.Generic;

namespace Abp.Auditing
{
    public interface IAbpAuditingDefaultOptions
    {
        /// <summary>
        /// A list of selectors to determine conventional Auditing classes.
        /// </summary>
        List<Func<Type, bool>> ConventionalAuditingSelectors { get; }
    }
}
