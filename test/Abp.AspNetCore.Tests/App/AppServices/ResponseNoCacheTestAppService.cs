using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.AppServices
{
    public class ResponseNoCacheTestAppService : ApplicationService
    {
        public string Get()
        {
            return "42";
        }

        [ResponseCache(Duration = 20, Location =ResponseCacheLocation.Client)]
        public string GetWithCache()
        {
            return "42";
        }
    }
}