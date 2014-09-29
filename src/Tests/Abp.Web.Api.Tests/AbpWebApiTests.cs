using Abp.Localization;

namespace Abp.Web.Api.Tests
{
    public static class AbpWebApiTests
    {
        private static AbpBootstrapper _bootstrapper;

        public static void Initialize()
        {
            if (_bootstrapper != null)
            {
                return;
            }
            
            _bootstrapper = new AbpBootstrapper();
            _bootstrapper.Initialize();
        }
    }
}