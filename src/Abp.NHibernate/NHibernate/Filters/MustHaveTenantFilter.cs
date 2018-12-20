using Abp.Domain.Entities;
using Abp.Domain.Uow;
using FluentNHibernate.Mapping;
using NHibernate;

namespace Abp.NHibernate.Filters
{
    /// <summary>
    /// Add filter MustHaveTenant 
    /// </summary>
    public class MustHaveTenantFilter : FilterDefinition
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MustHaveTenantFilter()
        {
            WithName(AbpDataFilters.MustHaveTenant)
                .AddParameter(AbpDataFilters.Parameters.TenantId, NHibernateUtil.Int32)
                .WithCondition($"{nameof(IMustHaveTenant.TenantId)} = :{AbpDataFilters.Parameters.TenantId}");
        }
    }
}