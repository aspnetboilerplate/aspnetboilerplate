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
            WithName("MayHaveTenant")
                .WithCondition("'1' IS NULL OR TenantId = '1' )");
        }
    }
}