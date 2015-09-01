using System.Threading.Tasks;
using System.Web.Http;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.WebApi.Controllers;

namespace Abp.WebApi.Runtime.Caching
{
    public class AbpCacheController : AbpApiController
    {
        private readonly ICacheProvider _cacheProvider;

        public AbpCacheController(ICacheProvider cacheProvider)
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

            if (model.Caches.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Caches can not be null or empty!");
            }

            await CheckPassword(model.Password);

            foreach (var cache in model.Caches)
            {
                await _cacheProvider.ClearCacheStoreAsync(cache);
            }
        }

        [HttpPost]
        public async Task ClearAll(ClearAllCacheModel model)
        {
            if (model.Password.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Password can not be null or empty!");
            }

            await CheckPassword(model.Password);

            await _cacheProvider.ClearAllCacheStoresAsync();
        }

        private async Task CheckPassword(string password)
        {
            var actualPassword = await SettingManager.GetSettingValueAsync(ClearCacheSettingNames.Password);
            if (actualPassword != password)
            {
                throw new UserFriendlyException("Password is not correct!");
            }
        }
    }
}
