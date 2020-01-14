using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;

namespace Abp.Webhooks
{
    /// <summary>
    /// Implements <see cref="IWebhookSendAttemptStore"/> using repositories.
    /// </summary>
    public class WebhookSendAttemptStore : IWebhookSendAttemptStore, ITransientDependency
    {
        private readonly IRepository<WebhookSendAttempt, Guid> _webhookSendAttemptRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public WebhookSendAttemptStore(IRepository<WebhookSendAttempt, Guid> webhookSendAttemptRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookSendAttemptRepository = webhookSendAttemptRepository;
            _unitOfWorkManager = unitOfWorkManager;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        [UnitOfWork]
        public virtual async Task InsertAsync(WebhookSendAttempt webhookSendAttempt)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
            {
                await _webhookSendAttemptRepository.InsertAsync(webhookSendAttempt);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void Insert(WebhookSendAttempt webhookSendAttempt)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
            {
                _webhookSendAttemptRepository.Insert(webhookSendAttempt);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task UpdateAsync(WebhookSendAttempt webhookSendAttempt)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
            {
                await _webhookSendAttemptRepository.UpdateAsync(webhookSendAttempt);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        [UnitOfWork]
        public virtual void Update(WebhookSendAttempt webhookSendAttempt)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
            {
                _webhookSendAttemptRepository.Update(webhookSendAttempt);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        [UnitOfWork]
        public virtual async Task<WebhookSendAttempt> GetAsync(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await _webhookSendAttemptRepository.GetAsync(id);
            }
        }

        [UnitOfWork]
        public virtual WebhookSendAttempt Get(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookSendAttemptRepository.Get(id);
            }
        }

        [UnitOfWork]
        public virtual async Task<int> GetSendAttemptCountAsync(int? tenantId, Guid webhookId, Guid webhookSubscriptionId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await AsyncQueryableExecuter.CountAsync(
                    _webhookSendAttemptRepository.GetAll()
                        .Where(attempt =>
                            attempt.WebhookId == webhookId &&
                            attempt.WebhookSubscriptionId == webhookSubscriptionId
                        )
                );
            }
        }

        [UnitOfWork]
        public virtual int GetSendAttemptCount(int? tenantId, Guid webhookId, Guid webhookSubscriptionId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookSendAttemptRepository.GetAll()
                    .Count(attempt =>
                        attempt.WebhookId == webhookId &&
                        attempt.WebhookSubscriptionId == webhookSubscriptionId);
            }
        }

        [UnitOfWork]
        public async Task<bool> HasAnySuccessfulAttemptInLastXRecordAsync(int? tenantId, Guid subscriptionId, int searchCount)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                if (await _webhookSendAttemptRepository.CountAsync(x => x.WebhookSubscriptionId == subscriptionId) < searchCount)
                {
                    return true;
                }

                return await AsyncQueryableExecuter.AnyAsync(_webhookSendAttemptRepository.GetAll().OrderByDescending(attempt => attempt.CreationTime).Take(searchCount)
                    .Where(x => x.ResponseStatusCode == HttpStatusCode.OK));
            }
        }

        [UnitOfWork]
        public bool HasAnySuccessfulAttemptInLastXRecord(int? tenantId, Guid subscriptionId, int searchCount)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                if (_webhookSendAttemptRepository.Count(x => x.WebhookSubscriptionId == subscriptionId) < searchCount)
                {
                    return true;
                }

                return _webhookSendAttemptRepository.GetAll().Where(x => x.WebhookSubscriptionId == subscriptionId).OrderByDescending(attempt => attempt.CreationTime).Take(searchCount)
                    .Any(x => x.ResponseStatusCode == HttpStatusCode.OK);
            }
        }
    }
}
