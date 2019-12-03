using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Linq;

namespace Abp.EntityHistory
{
    public abstract class EntitySnapshotManagerBase : IEntitySnapshotManager, ITransientDependency
    {
        protected readonly IRepository<EntityChange, long> EntityChangeRepository;
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        protected EntitySnapshotManagerBase(IRepository<EntityChange, long> entityChangeRepository)
        {
            EntityChangeRepository = entityChangeRepository;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        protected abstract Task<TEntity> GetEntityById<TEntity, TPrimaryKey>(TPrimaryKey id)
           where TEntity : class, IEntity<TPrimaryKey>;

        protected abstract IQueryable<EntityChange> GetEntityChanges<TEntity, TPrimaryKey>(TPrimaryKey id, DateTime snapshotTime)
           where TEntity : class, IEntity<TPrimaryKey>;

        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity, TPrimaryKey>(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var leftExpression = Expression.PropertyOrField(lambdaParam, "Id");

            var idValue = Convert.ChangeType(id, typeof(TPrimaryKey));

            Expression<Func<object>> closure = () => idValue;
            var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

            var lambdaBody = Expression.Equal(leftExpression, rightExpression);

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        public virtual async Task<EntityHistorySnapshot> GetSnapshotAsync<TEntity, TPrimaryKey>(TPrimaryKey id, DateTime snapshotTime)
           where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = await GetEntityById<TEntity, TPrimaryKey>(id);

            var snapshotPropertiesDictionary = new Dictionary<string, string>();
            var propertyChangesStackTreeDictionary = new Dictionary<string, string>();

            if (entity == null)
            {
                return new EntityHistorySnapshot(snapshotPropertiesDictionary, propertyChangesStackTreeDictionary);
            }

            var changes = await AsyncQueryableExecuter.ToListAsync(
                GetEntityChanges<TEntity, TPrimaryKey>(id, snapshotTime)
                    .Select(x => new { x.ChangeType, x.PropertyChanges })
            );

            //revoke all changes
            foreach (var change in changes)// desc ordered changes
            {
                foreach (var entityPropertyChange in change.PropertyChanges)
                {
                    RevokeChange<TEntity, TPrimaryKey>(snapshotPropertiesDictionary, entityPropertyChange, entity);

                    AddChangeToPropertyChangesStackTree<TEntity, TPrimaryKey>(
                        entityPropertyChange,
                        propertyChangesStackTreeDictionary,
                        entity
                    );
                }
            }

            return new EntityHistorySnapshot(snapshotPropertiesDictionary, propertyChangesStackTreeDictionary);
        }

        private static void RevokeChange<TEntity, TPrimaryKey>(
            Dictionary<string, string> snapshotPropertiesDictionary,
            EntityPropertyChange entityPropertyChange,
            TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            snapshotPropertiesDictionary[entityPropertyChange.PropertyName] = entityPropertyChange.OriginalValue;
        }

        private static void AddChangeToPropertyChangesStackTree<TEntity, TPrimaryKey>(
            EntityPropertyChange entityPropertyChange, 
            Dictionary<string, string> propertyChangesStackTreeDictionary, 
            TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (propertyChangesStackTreeDictionary.ContainsKey(entityPropertyChange.PropertyName))
            {
                propertyChangesStackTreeDictionary[entityPropertyChange.PropertyName] += " -> " + entityPropertyChange.OriginalValue;
            }
            else
            {
                var propertyCurrentValue = "PropertyNotExist";

                var propertyInfo = typeof(TEntity).GetProperty(entityPropertyChange.PropertyName);
                if (propertyInfo != null)
                {
                    var val = propertyInfo.GetValue(entity);
                    propertyCurrentValue = val == null ? "null" : val.ToJsonString();
                }

                propertyChangesStackTreeDictionary.Add(
                    entityPropertyChange.PropertyName,
                    propertyCurrentValue + " -> " + entityPropertyChange.OriginalValue
                );
            }
        }
    }
}
