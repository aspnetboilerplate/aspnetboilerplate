using System;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using RefactorThis.GraphDiff;

namespace Abp.GraphDiff.Mapping
{
    //TODO@Alexnader: write comments
    public static class MappingExpressionBuilder
    {
        public static EntityMapping For<TEntity>(Expression<Func<IUpdateConfiguration<TEntity>, object>> expression)
            where TEntity : class, IEntity
        {
            return For<TEntity, int>(expression);
        }

        public static EntityMapping For<TEntity, TPrimaryKey>(Expression<Func<IUpdateConfiguration<TEntity>, object>> expression)
            where TPrimaryKey : IEquatable<TPrimaryKey>
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return new EntityMapping(typeof(TEntity), expression);
        }
    }
}
