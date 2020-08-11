using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using IdentityServer4.Models;
using IdentityServer4.Stores;

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
            var predicate = Filter(filter);
            var entities = await _persistedGrantRepository.GetAllListAsync(predicate);

            return ObjectMapper.Map<List<PersistedGrant>>(entities);
        }

        [UnitOfWork]
        public virtual async Task RemoveAsync(string key)
        {
            await _persistedGrantRepository.DeleteAsync(key);
        }

        [UnitOfWork]
        public virtual async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            var predicate = Filter(filter);
            var entities = await _persistedGrantRepository.GetAllListAsync(predicate);

            Logger.DebugFormat("removing {persistedGrantCount} persisted grants from database for {@filter}", entities.Count, filter);
            Logger.InfoFormat("removing {persistedGrantCount} persisted grants from database for {@filter}", entities.Count, filter);

            foreach (var e in entities)
            {
                await _persistedGrantRepository.DeleteAsync(e.Id);
            }
        }

        private ExpressionStarter<PersistedGrantEntity> Filter(PersistedGrantFilter filter)
        {
            var predicate = PredicateBuilder.New<PersistedGrantEntity>();

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                predicate = predicate.And(x => x.ClientId == filter.ClientId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                predicate = predicate.And(x => x.SessionId == filter.SessionId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                predicate = predicate.And(x => x.SubjectId == filter.SubjectId);
            }
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                predicate = predicate.And(x => x.Type == filter.Type);
            }

            return predicate;
        }
    }
}
