using FluentNHibernate.Mapping;
using NHibernate;

namespace Abp.NHibernate.Filters
{
    public class MustHaveTenantFilter : FilterDefinition
    {
        public MustHaveTenantFilter()
        {
            WithName("MustHaveTenant")
                .WithCondition(":TenantId IS NOT NULL OR TenantId == :TenantId )")
                .AddParameter("TenantId", NHibernateUtil.Int64);
        }
    }
}