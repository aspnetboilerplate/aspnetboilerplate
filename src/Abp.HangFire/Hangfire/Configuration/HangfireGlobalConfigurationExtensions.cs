using System;
using Abp.Dependency;
using Hangfire;
using Hangfire.Annotations;

namespace Abp.Hangfire.Configuration
{
    public static class HangfireGlobalConfigurationExtensions
    {
        public static IGlobalConfiguration<WindsorJobActivator> UseWindsorJobActivator(
            [NotNull] this IGlobalConfiguration configuration,
            [NotNull]  IIocResolver iocResolver)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (iocResolver == null)
            {
                throw new ArgumentNullException("iocResolver");
            }

            return configuration.UseActivator(new WindsorJobActivator(iocResolver));
        }
    }
}