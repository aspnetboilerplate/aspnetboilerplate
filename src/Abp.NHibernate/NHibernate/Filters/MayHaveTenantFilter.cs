using Abp.Domain.Uow;
using FluentNHibernate.Mapping;
using NHibernate;

namespace Abp.NHibernate.Filters
{
    /// <summary>
    /// Add filter MayHaveTenant 
    /// </summary>
    public class MayHaveTenantFilter : FilterDefinition
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public MayHaveTenantFilter()
        {
            WithName(AbpDataFilters.MayHaveTenant)
                .AddParameter("tenantId", NHibernateUtil.Int32)
                .WithCondition("TenantId = :tenantId");
        }
    }
}