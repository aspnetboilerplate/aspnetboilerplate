using System;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using RefactorThis.GraphDiff;

namespace Abp.EntityFramework.GraphDiff.Mapping
{
    /// <summary>
    /// Helper class for creating entity mappings
    /// </summary>
    public static class MappingExpressionBuilder
    {
        /// <summary>
        /// A shortcut of <see cref="For{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static EntityMapping For<TEntity>(Expression<Func<IUpdateConfiguration<TEntity>, object>> expression)
            where TEntity : class, IEntity
        {
            return For<TEntity, int>(expression);
        }

        /// <summary>
        /// Build a mapping for an entity with a specified primary key
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static EntityMapping For<TEntity, TPrimaryKey>(Expression<Func<IUpdateConfiguration<TEntity>, object>> expression)
            where TPrimaryKey : IEquatable<TPrimaryKey>
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return new EntityMapping(typeof(TEntity), expression);
        }
    }
}
