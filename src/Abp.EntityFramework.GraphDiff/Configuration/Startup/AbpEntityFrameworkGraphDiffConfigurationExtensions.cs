using Abp.GraphDiff.Configuration.Startup;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP GraphDiff module.
    /// </summary>
    public static class AbpEntityFrameworkGraphDiffConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP GraphDiff module.
        /// </summary>
        public static IAbpEntityFrameworkGraphDiffModuleConfiguration AbpEfGraphDiff(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate(
                "Modules.Abp.EntityFramework.GraphDiff",
                () => configurations.AbpConfiguration.IocManager.Resolve<IAbpEntityFrameworkGraphDiffModuleConfiguration>());
        }
    }
}
