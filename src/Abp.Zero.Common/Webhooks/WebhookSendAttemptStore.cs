using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
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
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;

        public WebhookSendAttemptStore(
            IRepository<WebhookSendAttempt, Guid> webhookSendAttemptRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _webhookSendAttemptRepository = webhookSendAttemptRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _asyncQueryableExecuter = asyncQueryableExecuter;
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
                return await _webhookSendAttemptRepository
                    .CountAsync(attempt =>
                        attempt.WebhookEventId == webhookId &&
                        attempt.WebhookSubscriptionId == webhookSubscriptionId
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
                        attempt.WebhookEventId == webhookId &&
                        attempt.WebhookSubscriptionId == webhookSubscriptionId);
            }
        }

        [UnitOfWork]
        public async Task<bool> HasXConsecutiveFailAsync(int? tenantId, Guid subscriptionId, int failCount)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                if (await _webhookSendAttemptRepository.CountAsync(x => x.WebhookSubscriptionId == subscriptionId) < failCount)
                {
                    return false;
                }

                return !await _asyncQueryableExecuter.AnyAsync(
                    _webhookSendAttemptRepository.GetAll()
                        .OrderByDescending(attempt => attempt.CreationTime)
                        .Take(failCount)
                        .Where(attempt => attempt.ResponseStatusCode == HttpStatusCode.OK)
                );
            }
        }

        [UnitOfWork]
        public bool HasXConsecutiveFail(int? tenantId, Guid subscriptionId, int failCount)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                if (_webhookSendAttemptRepository.Count(x => x.WebhookSubscriptionId == subscriptionId) < failCount)
                {
                    return false;
                }

                return !_webhookSendAttemptRepository.GetAll()
                    .Where(attempt => attempt.WebhookSubscriptionId == subscriptionId)
                    .OrderByDescending(attempt => attempt.CreationTime)
                    .Take(failCount)
                    .Any(attempt => attempt.ResponseStatusCode == HttpStatusCode.OK);
            }
        }

        [UnitOfWork]
        public async Task<IPagedResult<WebhookSendAttempt>> GetAllSendAttemptsBySubscriptionAsPagedListAsync(int? tenantId, Guid subscriptionId, int maxResultCount, int skipCount)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = _webhookSendAttemptRepository.GetAllIncluding(attempt => attempt.WebhookEvent)
                    .Where(attempt =>
                        attempt.WebhookSubscriptionId == subscriptionId
                    );

                var totalCount = await _asyncQueryableExecuter.CountAsync(query);

                var list = await _asyncQueryableExecuter.ToListAsync(query
                    .OrderByDescending(attempt => attempt.CreationTime)
                    .Skip(skipCount)
                    .Take(maxResultCount)
                );

                return new PagedResultDto<WebhookSendAttempt>()
                {
                    TotalCount = totalCount,
                    Items = list
                };
            }
        }

        [UnitOfWork]
        public IPagedResult<WebhookSendAttempt> GetAllSendAttemptsBySubscriptionAsPagedList(int? tenantId, Guid subscriptionId, int maxResultCount, int skipCount)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = _webhookSendAttemptRepository.GetAllIncluding(attempt => attempt.WebhookEvent)
                    .Where(attempt =>
                        attempt.WebhookSubscriptionId == subscriptionId
                    );

                var totalCount = query.Count();

                var list = query
                    .OrderByDescending(attempt => attempt.CreationTime)
                    .Skip(skipCount)
                    .Take(maxResultCount)
                    .ToList();

                return new PagedResultDto<WebhookSendAttempt>()
                {
                    TotalCount = totalCount,
                    Items = list
                };
            }
        }

        [UnitOfWork]
        public Task<List<WebhookSendAttempt>> GetAllSendAttemptsByWebhookEventIdAsync(int? tenantId, Guid webhookEventId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _asyncQueryableExecuter.ToListAsync(
                    _webhookSendAttemptRepository.GetAll().Where(attempt => attempt.WebhookEventId == webhookEventId)
                        .OrderByDescending(attempt => attempt.CreationTime)
                );
            }
        }

        [UnitOfWork]
        public List<WebhookSendAttempt> GetAllSendAttemptsByWebhookEventId(int? tenantId, Guid webhookEventId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookSendAttemptRepository.GetAll().Where(attempt => attempt.WebhookEventId == webhookEventId)
                    .OrderByDescending(attempt => attempt.CreationTime).ToList();
            }
        }
    }
}
