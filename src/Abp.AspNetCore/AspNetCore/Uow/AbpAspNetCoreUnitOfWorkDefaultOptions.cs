using System;
using System.Collections.Generic;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.AspNetCore.Uow
{
    internal class AbpAspNetCoreUnitOfWorkDefaultOptions : UnitOfWorkDefaultOptions
    {
        public AbpAspNetCoreUnitOfWorkDefaultOptions()
        {
            ConventionalUowSelectors = new List<Func<Type, bool>>();
        }
    }
}
