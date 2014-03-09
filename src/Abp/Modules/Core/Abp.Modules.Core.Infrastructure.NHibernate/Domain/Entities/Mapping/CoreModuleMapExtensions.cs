using Abp.Domain.Entities.Auditing;
using FluentNHibernate.Mapping;

namespace Abp.Domain.Entities.Mapping
{
    /// <summary>
    /// This class is used to make mapping easier for standart columns.
    /// </summary>
    public static class CoreModuleMapExtensions
    {
        /// <summary>
        /// Maps audit columns. See <see cref="IAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapAudited<TEntity>(this ClassMap<TEntity> mapping) where TEntity : IAudited
        {
            mapping.MapCreationAudited();
            mapping.MapModificationAudited();
        }

        /// <summary>
        /// Maps creation audit columns. See <see cref="ICreationAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapCreationAudited<TEntity>(this ClassMap<TEntity> mapping) where TEntity : ICreationAudited
        {
            mapping.MapCreationTime();
            mapping.Map(x => (x as ICreationAudited).CreatorUserId);
        }

        /// <summary>
        /// Maps creation audit columns. See <see cref="ICreationAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapCreationTime<TEntity>(this ClassMap<TEntity> mapping) where TEntity : IHasCreationTime
        {
            mapping.Map(x => (x as IHasCreationTime).CreationTime);
        }

        /// <summary>
        /// Maps modification audit columns. See <see cref="ICreationAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapModificationAudited<TEntity>(this ClassMap<TEntity> mapping) where TEntity : IModificationAudited
        {
            mapping.Map(x => (x as IModificationAudited).LastModificationTime);
            mapping.Map(x => (x as IModificationAudited).LastModifierUserId);
        }
    }
}