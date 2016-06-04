using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Localization;

namespace Abp.GraphDiff.Mapping
{
    //TODO@Alexander: set a a fluent API module configuration instead of using SettingProvider?
    /// <summary>
    /// Defines entity mappings for the GraphDiff extension methods.
    /// Warn: use <see cref="GetEntityMappings" /> to set up the mappings.
    /// </summary>
    public abstract class EntityMappingProvider : SettingProvider
    {
        internal static string GraphDiffSettingGroupName = "Abp.EntityFramework.GraphDiff.Mappings";
        
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return GetEntityMappings()
                .Select(mapping => new SettingDefinition(
                    name: string.Format("Mapping for {0}", mapping.EntityType.FullName),
                    defaultValue: mapping.EntityType.FullName,
                    customData: mapping.MappingExpression,
                    group: new SettingDefinitionGroup(GraphDiffSettingGroupName, new FixedLocalizableString(GraphDiffSettingGroupName)))
                ).ToArray();
        }

        public virtual IEnumerable<EntityMapping> GetEntityMappings()
        {
            return new List<EntityMapping>();
        }
    }
}