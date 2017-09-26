using System;
using System.Linq;
using System.Linq.Expressions;
using Abp.Dependency;
using Abp.EntityFramework.GraphDiff.Configuration;
using RefactorThis.GraphDiff;

namespace Abp.EntityFramework.GraphDiff.Mapping
{
    /// <summary>
    /// Used for resolving mappings for a GraphDiff repository extension methods
    /// </summary>
    public class EntityMappingManager : IEntityMappingManager, ITransientDependency
    {
        private readonly IAbpEntityFrameworkGraphDiffModuleConfiguration _moduleConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntityMappingManager(IAbpEntityFrameworkGraphDiffModuleConfiguration moduleConfiguration)
        {
            _moduleConfiguration = moduleConfiguration;
        }

        /// <summary>
        /// Gets an entity mapping or null for a specified entity type
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Entity mapping or null if mapping doesn't exist</returns>
        public Expression<Func<IUpdateConfiguration<TEntity>, object>> GetEntityMappingOrNull<TEntity>()
        {
            var entityMapping = _moduleConfiguration.EntityMappings.FirstOrDefault(m => m.EntityType == typeof(TEntity));
            var mappingExptession = entityMapping?.MappingExpression as Expression<Func<IUpdateConfiguration<TEntity>, object>>;
            return mappingExptession;
        }
    }
}