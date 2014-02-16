using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FluentNHibernate.Mapping;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    /// <summary>
    /// This class is used to make mapping easier for standart columns.
    /// </summary>
    public static class MapExtensions
    {
        /// <summary>
        /// Maps audit columns. See <see cref="IAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapAuditColumns<TEntity>(this ClassMap<TEntity> mapping)
        {
            mapping.MapCreationAuditColumns();
            mapping.MapModificationAuditColumns();
        }

        /// <summary>
        /// Maps creation audit columns. See <see cref="ICreationAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapCreationAuditColumns<TEntity>(this ClassMap<TEntity> mapping)
        {
            mapping.Map(x => (x as ICreationAudited).CreationTime);
            mapping.References(x => (x as ICreationAudited).CreatorUser).Column("CreatorUserId").LazyLoad();
        }

        /// <summary>
        /// Maps modification audit columns. See <see cref="ICreationAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapModificationAuditColumns<TEntity>(this ClassMap<TEntity> mapping)
        {
            mapping.Map(x => (x as IModificationAudited).LastModificationTime);
            mapping.References(x => (x as IModificationAudited).LastModifierUser).Column("LastModifierUserId").LazyLoad();
        }
    }
}