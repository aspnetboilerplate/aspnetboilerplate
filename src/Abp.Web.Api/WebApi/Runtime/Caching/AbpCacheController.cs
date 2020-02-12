using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Web.Models;
using Abp.WebApi.Controllers;

namespace Abp.WebApi.Runtime.Caching
{
    [DontWrapResult]
    public class AbpCacheController : AbpApiController
    {
        private readonly ICacheManager _cacheManager;

        public AbpCacheController(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        [HttpPost]
        public async Task<AjaxResponse> Clear(ClearCacheModel model)
        {
            if (model.Password.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Password can not be null or empty!");
            }

            if (model.Caches.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Caches can not be null or empty!");
            }

            await CheckPasswordAsync(model.Password);

            var caches = _cacheManager.GetAllCaches().Where(c => model.Caches.Contains(c.Name));
            foreach (var cache in caches)
            {
                await cache.ClearAsync();
            }

            return new AjaxResponse();
        }

        [HttpPost]
        [Route("api/AbpCache/ClearAll")]
        public async Task<AjaxResponse> ClearAll(ClearAllCacheModel model)
        {
            if (model.Password.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Password can not be null or empty!");
            }

            await CheckPasswordAsync(model.Password);

            var caches = _cacheManager.GetAllCaches();
            foreach (var cache in caches)
            {
                await cache.ClearAsync();
            }

            return new AjaxResponse();
        }

        private async Task CheckPasswordAsync(string password)
        {
            var actualPassword = await SettingManager.GetSettingValueAsync(ClearCacheSettingNames.Password);
            if (actualPassword != password)
            {
                throw new UserFriendlyException("Password is not correct!");
            }
        }

        private void CheckPassword(string password)
        {
            var actualPassword = SettingManager.GetSettingValue(ClearCacheSettingNames.Password);
            if (actualPassword != password)
            {
                throw new UserFriendlyException("Password is not correct!");
            }
        }
    }
}
