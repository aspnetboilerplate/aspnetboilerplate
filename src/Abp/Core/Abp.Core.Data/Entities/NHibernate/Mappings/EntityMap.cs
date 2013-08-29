using System;
using FluentNHibernate.Mapping;

namespace Abp.Entities.NHibernate.Mappings
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
            if (string.IsNullOrWhiteSpace(tableName)) //TODO: Use code contracts or make a simple Helper?
            {
                throw new ArgumentNullException("tableName");
            }

            Table(tableName);
            Id(x => x.Id);

            if (typeof(ICreationAudited).IsAssignableFrom(typeof(TEntity)))
            {
                this.MapCreationAuditColumns();
            }

            if (typeof(IModificationAudited).IsAssignableFrom(typeof(TEntity)))
            {
                this.MapModificationAuditColumns();
            }
        }
    }

    /// <summary>
    /// A shortcut of <see cref="EntityMap{TEntity,TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    /// <typeparam name="TEntity">Entity map</typeparam>
    public abstract class EntityMap<TEntity> : EntityMap<TEntity, int> where TEntity : IEntity<int>
    {
        protected EntityMap(string tableName) : base(tableName)
        {

        }
    }
}
