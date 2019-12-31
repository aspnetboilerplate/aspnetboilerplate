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

        public WebHookStore(IRepository<WebHookInfo, Guid> webHookRepository)
        {
            _webHookRepository = webHookRepository;
        }

        [UnitOfWork]
        public Task<Guid> InsertAndGetIdAsync(WebHookInfo webHookInfo)
        {
            return _webHookRepository.InsertAndGetIdAsync(webHookInfo);
        }

        [UnitOfWork]
        public Guid InsertAndGetId(WebHookInfo webHookInfo)
        {
            return _webHookRepository.InsertAndGetId(webHookInfo);
        }

        [UnitOfWork]
        public Task<WebHookInfo> GetAsync(Guid id)
        {
            return _webHookRepository.GetAsync(id);
        }

        [UnitOfWork]
        public WebHookInfo Get(Guid id)
        {
            return _webHookRepository.Get(id);
        }
    }
}
