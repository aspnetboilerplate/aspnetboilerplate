using System;
using Adorable.Domain.Entities;
using Adorable.NHibernate.Filters;
using Adorable.Runtime.Session;
using FluentNHibernate.Mapping;

namespace Adorable.NHibernate.EntityMappings
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
            if (string.IsNullOrWhiteSpace(tableName)) //TODO: Use code contracts?
            {
                throw new ArgumentNullException("tableName");
            }

            Table(tableName);
            Id(x => x.Id);

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Where("IsDeleted = 0"); //TODO: Test with other DBMS then SQL Server
            }

            if (typeof(IMustHaveTenant).IsAssignableFrom(typeof (TEntity)))
                ApplyFilter<MustHaveTenantFilter>();
            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
                ApplyFilter<MayHaveTenantFilter>();


        }
    }
}