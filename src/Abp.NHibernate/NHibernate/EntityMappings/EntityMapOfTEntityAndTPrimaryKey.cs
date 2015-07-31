using System;
using Abp.Domain.Entities;
using Abp.NHibernate.Filters;
using Abp.Runtime.Session;
using FluentNHibernate.Mapping;

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