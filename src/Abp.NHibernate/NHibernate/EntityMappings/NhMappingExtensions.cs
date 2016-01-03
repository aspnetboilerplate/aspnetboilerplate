using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FluentNHibernate.Mapping;

namespace Abp.NHibernate.EntityMappings
{
    /// <summary>
    /// This class is used to make mapping easier for standart columns.
    /// </summary>
    public static class NhMappingExtensions
    {
        /// <summary>
        /// Maps full audit columns (defined by <see cref="IFullAudited"/>).
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapFullAudited<TEntity>(this ClassMap<TEntity> mapping)
            where TEntity : IFullAudited
        {
            mapping.MapAudited();
            mapping.MapDeletionAudited();
        }

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
            mapping.Map(x => x.CreatorUserId);
        }

        /// <summary>
        /// Maps CreationTime column. See <see cref="ICreationAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapCreationTime<TEntity>(this ClassMap<TEntity> mapping) where TEntity : IHasCreationTime
        {
            mapping.Map(x => x.CreationTime);
        }

        /// <summary>
        /// Maps LastModificationTime column. See <see cref="IHasModificationTime"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapLastModificationTime<TEntity>(this ClassMap<TEntity> mapping) where TEntity : IHasModificationTime
        {
            mapping.Map(x => x.LastModificationTime);
        }

        /// <summary>
        /// Maps modification audit columns. See <see cref="IModificationAudited"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapModificationAudited<TEntity>(this ClassMap<TEntity> mapping) where TEntity : IModificationAudited
        {
            mapping.MapLastModificationTime();
            mapping.Map(x => x.LastModifierUserId);
        }

        /// <summary>
        /// Maps deletion audit columns (defined by <see cref="IDeletionAudited"/>).
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapDeletionAudited<TEntity>(this ClassMap<TEntity> mapping) where TEntity : IDeletionAudited
        {
            mapping.MapIsDeleted();
            mapping.Map(x => x.DeleterUserId);
            mapping.Map(x => x.DeletionTime);
        }

        /// <summary>
        /// Maps IsDeleted column (defined by <see cref="ISoftDelete"/>).
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapIsDeleted<TEntity>(this ClassMap<TEntity> mapping) where TEntity : ISoftDelete
        {
            mapping.Map(x => x.IsDeleted);
        }

        /// <summary>
        /// Maps MapIsActive column (defined by <see cref="IPassivable"/>).
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void MapIsActive<TEntity>(this ClassMap<TEntity> mapping) where TEntity : IPassivable
        {
            mapping.Map(x => x.IsActive);
        }
    }
}