using System;
using Abp.Startup.Configuration;
using Owin;

namespace Abp.Owin
{
    public static class AbpAppBuilderExtensions
    {
        public static void UseAbp(this IAppBuilder app)
        {
            UseAbp(app, configuration => { });
        }

        public static void UseAbp(this IAppBuilder app, Action<AbpConfiguration> configurationMethod)
        {
            configurationMethod(AbpConfiguration.Instance);
        }
    }
}
