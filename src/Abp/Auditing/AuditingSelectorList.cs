using System;
using System.Collections.Generic;

namespace Abp.Auditing
{
    internal class AuditingSelectorList : List<Func<Type, bool>>, IAuditingSelectorList
    {

    }
}