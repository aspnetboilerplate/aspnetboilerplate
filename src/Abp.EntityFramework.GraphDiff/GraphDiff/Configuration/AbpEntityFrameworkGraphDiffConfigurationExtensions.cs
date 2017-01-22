using System.Collections.Generic;
using System.Linq;
using Abp.Configuration.Startup;
using Abp.EntityFramework.GraphDiff.Mapping;

namespace Abp.EntityFramework.GraphDiff.Configuration
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
            return configurations.AbpConfiguration.Get<IAbpEntityFrameworkGraphDiffModuleConfiguration>();
        }

        /// <summary>
        /// Used to provide a mappings for the Abp.EntityFramework.GraphDiff module.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="entityMappings"></param>
        public static void UseMappings(this IAbpEntityFrameworkGraphDiffModuleConfiguration configuration, IEnumerable<EntityMapping> entityMappings)
        {
            configuration.EntityMappings = entityMappings.ToList();
        }
    }
}
