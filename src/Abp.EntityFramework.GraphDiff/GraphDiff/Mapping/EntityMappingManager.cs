using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Configuration;
using Abp.Dependency;
using RefactorThis.GraphDiff;
using System.Linq;

namespace Abp.GraphDiff.Mapping
{
    /// <summary>
    /// Used for resolving mappings for a GraphDiff repositroy extension methods
    /// </summary>
    internal class EntityMappingManager : IEntityMappingManager, ITransientDependency
    {
        private Dictionary<Type, object> _mappings;
        private readonly ISettingDefinitionManager _settingDefinitionManager;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public EntityMappingManager(ISettingDefinitionManager settingDefinitionManager)
        {
            _settingDefinitionManager = settingDefinitionManager;

            InitializeMappings();
        }

        /// <summary>
        /// Loads mappings using SettingDefinitionManager, using settings in group "<see cref="EntityMappingProvider.GraphDiffSettingGroupName"/>"
        /// </summary>
        private void InitializeMappings()
        {
            _mappings = new Dictionary<Type, object>();
            var allSettings = _settingDefinitionManager.GetAllSettingDefinitions();
            foreach (var setting in allSettings.Where(s => s.Group.Name == EntityMappingProvider.GraphDiffSettingGroupName))
            {
                var type = Type.GetType(setting.DefaultValue);
                if (type != null && !_mappings.ContainsKey(type))
                {
                    _mappings.Add(
                        type,
                        setting.CustomData as Expression<Func<IUpdateConfiguration<Type>, object>>);
                }
            }
        }

        /// <summary>
        /// Gets an entity mapping or null
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Entity mapping or null</returns>
        public Expression<Func<IUpdateConfiguration<TEntity>, object>> GetEntityMappingOrNull<TEntity>()
        {
            //Check wheter mapping exists
            if (_mappings.ContainsKey(typeof(TEntity)))
                return null;

            var mapping = (Expression<Func<IUpdateConfiguration<TEntity>, object>>)_mappings[typeof(TEntity)];
            return mapping;
        }
    }
}