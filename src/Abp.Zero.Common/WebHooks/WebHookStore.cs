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
        private readonly IRepository<WebhookInfo, Guid> _webhookRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WebhookStore(IRepository<WebhookInfo, Guid> webhookRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookRepository = webhookRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual async Task<Guid> InsertAndGetIdAsync(WebhookInfo webhookInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webhookInfo.TenantId))
            {
                var id = await _webhookRepository.InsertAndGetIdAsync(webhookInfo);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                return id;
            }
        }

        [UnitOfWork]
        public virtual Guid InsertAndGetId(WebhookInfo webhookInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webhookInfo.TenantId))
            {
                var id = _webhookRepository.InsertAndGetId(webhookInfo);
                _unitOfWorkManager.Current.SaveChanges();
                return id;
            }
        }

        [UnitOfWork]
        public virtual Task<WebhookInfo> GetAsync(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookRepository.GetAsync(id);
            }
        }

        [UnitOfWork]
        public virtual WebhookInfo Get(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webhookRepository.Get(id);
            }
        }
    }
}
