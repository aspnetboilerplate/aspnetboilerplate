using Abp.Domain.Entities;
using Abp.NHibernate.Filters;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Abp.NHibernate.EntityMappings
{
    /// <summary>
    /// This class is base class to map entities to database tables.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Type of primary key of the entity</typeparam>
    public abstract class EntityMap<TEntity, TPrimaryKey> : ClassMap<TEntity> where TEntity : IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        protected EntityMap(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            Table(tableName);
            Id(x => x.Id);

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                ApplyFilter<SoftDeleteFilter>();
            }

            if (typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                ApplyFilter<MustHaveTenantFilter>();
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                ApplyFilter<MayHaveTenantFilter>();
            }
        }

        protected new OneToManyPart<TChild> HasMany<TChild>(Expression<Func<TEntity, IEnumerable<TChild>>> memberExpression)
        {
            var mapping = base.HasMany<TChild>(memberExpression);
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TChild)))
            {
                mapping.ApplyFilter<SoftDeleteFilter>();
            }
            return mapping;
        }

        protected new OneToManyPart<TChild> HasMany<TKey, TChild>(Expression<Func<TEntity, IDictionary<TKey, TChild>>> memberExpression)
        {
            var mapping = base.HasMany<TKey, TChild>(memberExpression);
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TChild)))
            {
                mapping.ApplyFilter<SoftDeleteFilter>();
            }

            return mapping;
        }

        protected new OneToManyPart<TChild> HasMany<TChild>(Expression<Func<TEntity, object>> memberExpression)
        {
            var mapping = base.HasMany<TChild>(memberExpression);
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TChild)))
            {
                mapping.ApplyFilter<SoftDeleteFilter>();
            }

            return mapping;
        }

        protected new ManyToManyPart<TChild> HasManyToMany<TChild>(Expression<Func<TEntity, IEnumerable<TChild>>> memberExpression)
        {
            var mapping = base.HasManyToMany<TChild>(memberExpression);
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TChild)))
            {
                mapping.ApplyChildFilter<SoftDeleteFilter>();
            }

            return mapping;
        }

        protected new ManyToManyPart<TChild> HasManyToMany<TChild>(Expression<Func<TEntity, object>> memberExpression)
        {
            var mapping = base.HasManyToMany<TChild>(memberExpression);
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TChild)))
            {
                mapping.ApplyChildFilter<SoftDeleteFilter>();
            }

            return mapping;
        }
    }
}