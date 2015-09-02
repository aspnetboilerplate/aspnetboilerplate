using System.Linq;
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

            var cacheStores = _cacheProvider.GetAllCacheStores().Where(c => model.Caches.Contains(c.Name));
            foreach (var cacheStore in cacheStores)
            {
                await cacheStore.ClearAsync();
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

            var cacheStores = _cacheProvider.GetAllCacheStores();
            foreach (var cacheStore in cacheStores)
            {
                await cacheStore.ClearAsync();
            }
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
