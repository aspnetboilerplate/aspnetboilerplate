using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.WebHooks
{
    /// <summary>
    /// Implements <see cref="IWebHookStore"/> using repositories.
    /// </summary>
    public class WebHookStore : IWebHookStore, ITransientDependency
    {
        private readonly IRepository<WebHookInfo, Guid> _webHookRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WebHookStore(IRepository<WebHookInfo, Guid> webHookRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _webHookRepository = webHookRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual async Task<Guid> InsertAndGetIdAsync(WebHookInfo webHookInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webHookInfo.TenantId))
            {
                var id = await _webHookRepository.InsertAndGetIdAsync(webHookInfo);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                return id;
            }
        }

        [UnitOfWork]
        public virtual Guid InsertAndGetId(WebHookInfo webHookInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(webHookInfo.TenantId))
            {
                var id = _webHookRepository.InsertAndGetId(webHookInfo);
                _unitOfWorkManager.Current.SaveChanges();
                return id;
            }
        }

        [UnitOfWork]
        public virtual Task<WebHookInfo> GetAsync(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webHookRepository.GetAsync(id);
            }
        }

        [UnitOfWork]
        public virtual WebHookInfo Get(int? tenantId, Guid id)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return _webHookRepository.Get(id);
            }
        }
    }
}
