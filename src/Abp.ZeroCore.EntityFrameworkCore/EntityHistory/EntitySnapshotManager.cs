using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Json;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityHistory
{
    public class EntitySnapshotManager : IEntitySnapshotManager, ITransientDependency
    {
        private readonly IRepository<EntityChange, long> _entityChangeRepository;

        public EntitySnapshotManager(IRepository<EntityChange, long> entityChangeRepository)
        {
            _entityChangeRepository = entityChangeRepository;
        }
        public async Task<EntityHistorySnapshot> GetSnapshotAsync<TEntity>(int id, DateTime snapshotTime) where TEntity : class, IEntity<int>
        {
            return await GetSnapshotAsync<TEntity, int>(id, snapshotTime);
        }
        public async Task<EntityHistorySnapshot> GetSnapshotAsync<TEntity, TPrimaryKey>(TPrimaryKey id, DateTime snapshotTime) where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = _entityChangeRepository.GetDbContext()
                .Set<TEntity>().AsQueryable().FirstOrDefault(x => x.Id.Equals(id));

            var snapshotPropertiesDictionary = new Dictionary<string, string>();
            var propertyChangesStackTreeDictionary = new Dictionary<string, string>();

            if (entity != null)
            {
                string fullName = typeof(TEntity).FullName;
                var idJson = id.ToJsonString();

                var allChanges = _entityChangeRepository.GetAll().Include(x => x.PropertyChanges).ToList();
                var changes = await _entityChangeRepository.GetAll()//select all changes which created after snapshot time 
                    .Where(x => x.EntityTypeFullName == fullName && x.EntityId == idJson && x.ChangeTime > snapshotTime && x.ChangeType != EntityChangeType.Created)
                    .OrderByDescending(x => x.ChangeTime)
                    .Select(x => new { x.ChangeType, x.PropertyChanges }).ToListAsync();

                //and revoke all changes

                foreach (var change in changes)// desc ordered changes
                {
                    foreach (var entityPropertyChange in change.PropertyChanges)
                    {
                        if (snapshotPropertiesDictionary.ContainsKey(entityPropertyChange.PropertyName))
                        {
                            snapshotPropertiesDictionary[entityPropertyChange.PropertyName] = entityPropertyChange.OriginalValue;//set back to orginal value
                        }
                        else
                        {
                            snapshotPropertiesDictionary.Add(entityPropertyChange.PropertyName, entityPropertyChange.OriginalValue);//set back to orginal value
                        }

                        //create change stack tree
                        if (propertyChangesStackTreeDictionary.ContainsKey(entityPropertyChange.PropertyName))
                        {
                            propertyChangesStackTreeDictionary[entityPropertyChange.PropertyName] += " -> " + entityPropertyChange.OriginalValue;
                        }
                        else
                        {
                            string propertyCurrentValue = "PropertyNotExist";

                            var propertyInfo = typeof(TEntity).GetProperty(entityPropertyChange.PropertyName);
                            if (propertyInfo != null)
                            {
                                var val = propertyInfo.GetValue(entity);
                                if (val == null)
                                {
                                    propertyCurrentValue = "null";
                                }
                                else
                                {
                                    propertyCurrentValue = val.ToJsonString();
                                }
                            }
                            propertyChangesStackTreeDictionary.Add(entityPropertyChange.PropertyName, propertyCurrentValue + " -> " + entityPropertyChange.OriginalValue);
                        }
                    }
                }
            }
            return new EntityHistorySnapshot(snapshotPropertiesDictionary, propertyChangesStackTreeDictionary);
        }
    }
}
