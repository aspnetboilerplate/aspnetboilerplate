using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.Webhooks
{
    /// <summary>
    /// Implements <see cref="IWebhookStore"/> using repositories.
    /// </summary>
    public class WebhookStore : IWebhookStore, ITransientDependency
    {
        private readonly IRepository<WebhookEvent, Guid> _webhookRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WebhookStore(IRepository<WebhookEvent, Guid> webhookRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookRepository = webhookRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual async Task<Guid> InsertAndGetIdAsync(WebhookEvent webhookEvent)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webhookEvent.TenantId))
            {
                var id = await _webhookRepository.InsertAndGetIdAsync(webhookEvent);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                return id;
            }
        }

        [UnitOfWork]
        public virtual Guid InsertAndGetId(WebhookEvent webhookEvent)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webhookEvent.TenantId))
            {
                var id = _webhookRepository.InsertAndGetId(webhookEvent);
                _unitOfWorkManager.Current.SaveChanges();
                return id;
            }
        }

        [UnitOfWork]
        public virtual Task<WebhookEvent> GetAsync(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookRepository.GetAsync(id);
            }
        }

        [UnitOfWork]
        public virtual WebhookEvent Get(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookRepository.Get(id);
            }
        }
    }
}
