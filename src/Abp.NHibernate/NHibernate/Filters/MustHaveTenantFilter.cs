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
            WithName("MustHaveTenant")
                .WithCondition("TenantId = :Tenant")
                .AddParameter("Tenant", NHibernateUtil.Int64);
        }
    }
}