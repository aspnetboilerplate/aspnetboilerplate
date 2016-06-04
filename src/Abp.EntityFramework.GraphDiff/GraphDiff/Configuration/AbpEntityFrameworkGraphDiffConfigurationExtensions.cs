using Abp.Configuration.Startup;

namespace Abp.GraphDiff.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Abp.EntityFramework.GraphDiff module.
    /// </summary>
    public static class AbpEntityFrameworkGraphDiffConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Abp.EntityFramework.GraphDiff module.
        /// </summary>
        public static IAbpEntityFrameworkGraphDiffModuleConfiguration AbpEfGraphDiff(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate(
                "Modules.Abp.EntityFramework.GraphDiff",
                () => configurations.AbpConfiguration.IocManager.Resolve<IAbpEntityFrameworkGraphDiffModuleConfiguration>());
        }
    }
}
