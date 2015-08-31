using System.Threading.Tasks;
using System.Web.Http;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.WebApi.Controllers;

namespace Abp.WebApi.Runtime.Caching
{
    public class AbpCacheApiController : AbpApiController
    {
        private readonly ICacheProvider _cacheProvider;

        public AbpCacheApiController(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        [HttpPost]
        public async Task Clear(ClearCacheModel model)
        {
            if (model.Password.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Password can not be null or empty!");
            }

            //TODO: Define a password setting and check the password!

            if (model.Caches.IsNullOrEmpty())
            {
                await _cacheProvider.ClearAllCacheStoresAsync();
                return;
            }
            
            foreach (var cache in model.Caches)
            {
                await _cacheProvider.ClearCacheStoreAsync(cache);
            }
        }
    }
}
