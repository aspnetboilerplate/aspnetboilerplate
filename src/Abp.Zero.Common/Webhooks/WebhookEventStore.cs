using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.Webhooks
{
    /// <summary>
    /// Implements <see cref="IWebhookEventStore"/> using repositories.
    /// </summary>
    public class WebhookEventStore : IWebhookEventStore, ITransientDependency
    {
        private readonly IRepository<WebhookEvent, Guid> _webhookRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WebhookEventStore(
            IRepository<WebhookEvent, Guid> webhookRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookRepository = webhookRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual async Task<Guid> InsertAndGetIdAsync(WebhookEvent webhookEvent)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(webhookEvent.TenantId))
                {
                    var id = await _webhookRepository.InsertAndGetIdAsync(webhookEvent);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    return id;
                }
            });
        }

        public virtual Guid InsertAndGetId(WebhookEvent webhookEvent)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(webhookEvent.TenantId))
                {
                    var id = _webhookRepository.InsertAndGetId(webhookEvent);
                    _unitOfWorkManager.Current.SaveChanges();
                    return id;
                }
            });
        }

        public virtual async Task<WebhookEvent> GetAsync(int? tenantId, Guid id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    return await _webhookRepository.GetAsync(id);
                }
            });
        }

        public virtual WebhookEvent Get(int? tenantId, Guid id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    return _webhookRepository.Get(id);
                }
            });
        }
    }
}
