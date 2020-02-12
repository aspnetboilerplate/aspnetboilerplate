using Abp.Domain.Entities;
using Abp.Domain.Uow;
using FluentNHibernate.Mapping;
using NHibernate;

namespace Abp.NHibernate.Filters
{
    /// <summary>
    /// Add filter SoftDelete 
    /// </summary>
    public class SoftDeleteFilter : FilterDefinition
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SoftDeleteFilter()
        {
            WithName(AbpDataFilters.SoftDelete)
                .AddParameter(AbpDataFilters.Parameters.IsDeleted, NHibernateUtil.Boolean)
                .WithCondition($"{nameof(ISoftDelete.IsDeleted)} = :{AbpDataFilters.Parameters.IsDeleted}");
        }
    }
}