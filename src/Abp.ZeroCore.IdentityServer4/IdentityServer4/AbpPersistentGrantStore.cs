using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
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
        public virtual async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var entities = await _persistedGrantRepository.GetAllListAsync(x => x.SubjectId == subjectId);
            return ObjectMapper.Map<List<PersistedGrant>>(entities);
        }

        [UnitOfWork]
        public virtual async Task RemoveAsync(string key)
        {
            await _persistedGrantRepository.DeleteAsync(key);
        }

        [UnitOfWork]
        public virtual async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _persistedGrantRepository.DeleteAsync(x => x.SubjectId == subjectId && x.ClientId == clientId);
        }

        [UnitOfWork]
        public virtual async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _persistedGrantRepository.DeleteAsync(x => x.SubjectId == subjectId && x.ClientId == clientId && x.Type == type);
        }
    }
}
