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
        /// Contructor
        /// </summary>
        public MustHaveTenantFilter()
        {
            WithName(AbpDataFilters.MustHaveTenant)
                .AddParameter("tenantId", NHibernateUtil.Int32)
                .WithCondition("TenantId = :tenantId");
        }
    }
}