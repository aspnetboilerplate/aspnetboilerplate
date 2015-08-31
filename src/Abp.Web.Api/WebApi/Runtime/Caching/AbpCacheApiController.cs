using System.Threading.Tasks;
using System.Web.Http;
using Abp.Runtime.Caching;
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
            if (!ModelState.IsValid)
            {
                //TODO: throw exception!
            }

            //TODO: Check password!

            foreach (var cache in model.Caches)
            {
                await _cacheProvider.ClearCacheStoreAsync(cache);
            }
        }
    }
}
