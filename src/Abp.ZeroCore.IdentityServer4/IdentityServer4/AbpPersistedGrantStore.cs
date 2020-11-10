using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Abp.Extensions;

namespace Abp.IdentityServer4
{
    public class AbpPersistedGrantStore : AbpServiceBase, IPersistedGrantStore
    {
        private readonly IRepository<PersistedGrantEntity, string> _persistedGrantRepository;

        public AbpPersistedGrantStore(IRepository<PersistedGrantEntity, string> persistedGrantRepository)
        {
            _persistedGrantRepository = persistedGrantRepository;
        }

        [UnitOfWork]
        public virtual async Task StoreAsync(PersistedGrant grant)
        {
            var entity = await _persistedGrantRepository.FirstOrDefaultAsync(grant.Key);
            if (entity == null)
            {
                await _persistedGrantRepository.InsertAsync(ObjectMapper.Map<PersistedGrantEntity>(grant));
            }
            else
            {
                ObjectMapper.Map(grant, entity);
                await _persistedGrantRepository.UpdateAsync(entity);
            }
        }

        [UnitOfWork]
        public virtual async Task<PersistedGrant> GetAsync(string key)
        {
            var entity = await _persistedGrantRepository.FirstOrDefaultAsync(key);
            if (entity == null)
            {
                return null;
            }

            return ObjectMapper.Map<PersistedGrant>(entity);
        }

        [UnitOfWork]
        public virtual async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var entities = await _persistedGrantRepository.GetAllListAsync(FilterPersistedGrant(filter));
            return ObjectMapper.Map<List<PersistedGrant>>(entities);
        }

        [UnitOfWork]
        public virtual async Task RemoveAsync(string key)
        {
            await _persistedGrantRepository.DeleteAsync(key);
        }

        [UnitOfWork]
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            await _persistedGrantRepository.DeleteAsync(FilterPersistedGrant(filter));
        }

        protected virtual Expression<Func<PersistedGrantEntity, bool>> FilterPersistedGrant(PersistedGrantFilter filter)
        {
            var predicate = PredicateBuilder.New<PersistedGrantEntity>();

            if (!filter.SubjectId.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(x => x.SubjectId == filter.SubjectId);
            }

            if (!filter.SessionId.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(x => x.SessionId == filter.SessionId);
            }

            if (!filter.ClientId.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(x => x.ClientId == filter.ClientId);
            }

            if (!filter.Type.IsNullOrWhiteSpace())
            {
                predicate = predicate.And(x => x.Type == filter.Type);
            }

            return predicate;
        }
    }
}

