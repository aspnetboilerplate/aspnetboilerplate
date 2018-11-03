using Abp.Domain.Entities;
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
        /// Constructor
        /// </summary>
        public MayHaveTenantFilter()
        {
            WithName(AbpDataFilters.MayHaveTenant)
                .AddParameter(AbpDataFilters.Parameters.TenantId, NHibernateUtil.Int32)
                .WithCondition($"{nameof(IMayHaveTenant.TenantId)} = :{AbpDataFilters.Parameters.TenantId}");
        }
    }
}