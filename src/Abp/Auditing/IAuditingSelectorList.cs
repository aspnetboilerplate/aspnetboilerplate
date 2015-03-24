using System;
using System.Collections.Generic;

namespace Abp.Auditing
{
    /// <summary>
    /// List of selector functions to select classes/interfaces to be audited.
    /// </summary>
    public interface IAuditingSelectorList : IList<Func<Type, bool>>
    {

    }
}