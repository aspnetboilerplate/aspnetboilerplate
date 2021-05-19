using System;
using System.Collections.Generic;
using Abp.Application.Services;

namespace Abp.Auditing
{
    internal class AbpAuditingDefaultOptions : IAbpAuditingDefaultOptions
    {
        public List<Func<Type, bool>> ConventionalAuditingSelectors { get; protected set; }

        public AbpAuditingDefaultOptions()
        {
            ConventionalAuditingSelectors = new List<Func<Type, bool>>
            {
                type => typeof(IApplicationService).IsAssignableFrom(type)
            };
        }
    }
}