using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;

namespace Abp.WebHooks
{
    /// <summary>
    /// Implements <see cref="IWebHookSubscriptionsStore"/> using repositories.
    /// </summary>
    public class WebHookSubscriptionsStore : IWebHookSubscriptionsStore, ITransientDependency
    {
        private readonly IRepository<WebHookSubscriptionInfo, Guid> _webhookSubscriptionRepository;

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public WebHookSubscriptionsStore(IRepository<WebHookSubscriptionInfo, Guid> webhookSubscriptionRepository)
        {
            _webhookSubscriptionRepository = webhookSubscriptionRepository;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        [UnitOfWork]
        public Task<WebHookSubscriptionInfo> GetAsync(Guid id)
        {
            return _webhookSubscriptionRepository.GetAsync(id);
        }

        [UnitOfWork]
        public WebHookSubscriptionInfo Get(Guid id)
        {
            return _webhookSubscriptionRepository.Get(id);
        }

        [UnitOfWork]
        public Task InsertAsync(WebHookSubscriptionInfo webHookInfo)
        {
            return _webhookSubscriptionRepository.InsertAsync(webHookInfo);
        }

        [UnitOfWork]
        public void Insert(WebHookSubscriptionInfo webHookInfo)
        {
            _webhookSubscriptionRepository.Insert(webHookInfo);
        }

        [UnitOfWork]
        public Task UpdateAsync(WebHookSubscriptionInfo webHookSubscription)
        {
            return _webhookSubscriptionRepository.UpdateAsync(webHookSubscription);
        }

        [UnitOfWork]
        public void Update(WebHookSubscriptionInfo webHookSubscription)
        {
            _webhookSubscriptionRepository.Update(webHookSubscription);
        }

        [UnitOfWork]
        public Task DeleteAsync(Guid id)
        {
            return _webhookSubscriptionRepository.DeleteAsync(id);
        }

        [UnitOfWork]
        public void Delete(Guid id)
        {
            _webhookSubscriptionRepository.Delete(id);
        }

        [UnitOfWork]
        public Task<List<WebHookSubscriptionInfo>> GetAllSubscriptionsAsync(string webHookDefinitionName)
        {
            return AsyncQueryableExecuter.ToListAsync(_webhookSubscriptionRepository.GetAll()
                .Where(s => s.WebHookDefinitions.Contains("\"" + webHookDefinitionName + "\"")));
        }

        [UnitOfWork]
        public List<WebHookSubscriptionInfo> GetAllSubscriptions(string webHookDefinitionName)
        {
            return _webhookSubscriptionRepository.GetAll()
                .Where(s => s.WebHookDefinitions.Contains("\"" + webHookDefinitionName + "\"")).ToList();
        }

        [UnitOfWork]
        public Task<bool> IsSubscribedAsync(UserIdentifier user, string webHookName)
        {
            return AsyncQueryableExecuter.AnyAsync(_webhookSubscriptionRepository.GetAll()
                .Where(s =>
                    s.IsActive &&
                    s.TenantId == user.TenantId &&
                    s.UserId == user.UserId &&
                    s.WebHookDefinitions.Contains("\"" + webHookName + "\"")
                ));
        }

        [UnitOfWork]
        public bool IsSubscribed(UserIdentifier user, string webHookName)
        {
            return _webhookSubscriptionRepository.GetAll()
                .Any(s =>
                    s.IsActive &&
                    s.TenantId == user.TenantId &&
                    s.UserId == user.UserId &&
                    s.WebHookDefinitions.Contains("\"" + webHookName + "\"")
                );
        }

        [UnitOfWork]
        public Task<List<WebHookSubscriptionInfo>> GetSubscribedWebHooksAsync(UserIdentifier user)
        {
            return AsyncQueryableExecuter.ToListAsync(_webhookSubscriptionRepository.GetAll()
                .Where(s => s.TenantId == user.TenantId && s.UserId == user.UserId));
        }

        [UnitOfWork]
        public List<WebHookSubscriptionInfo> GetSubscribedWebHooks(UserIdentifier user)
        {
            return _webhookSubscriptionRepository.GetAll().Where(s => s.TenantId == user.TenantId && s.UserId == user.UserId).ToList();
        }

        [UnitOfWork]
        public async Task SetActiveAsync(Guid id, bool active)
        {
            var subscription = await _webhookSubscriptionRepository.GetAsync(id);
            subscription.IsActive = true;
        }

        [UnitOfWork]
        public void SetActive(Guid id, bool active)
        {
            var subscription = _webhookSubscriptionRepository.Get(id);
            subscription.IsActive = true;
        }
    }
}
