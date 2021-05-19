using System;
using System.Collections.Generic;
using Abp.Auditing;

namespace Abp.AspNetCore.Mvc.Auditing
{
    internal class AbpAspNetCoreAuditingDefaultOptions: AbpAuditingDefaultOptions
    {
        public AbpAspNetCoreAuditingDefaultOptions()
        {
            ConventionalAuditingSelectors = new List<Func<Type, bool>>();
        }
    }
}
