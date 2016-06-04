using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abp.Configuration;
using Abp.Localization;
using RefactorThis.GraphDiff;

namespace Abp.GraphDiff.Mapping
{
    /// <summary>
    /// Defines entity mappings for the GraphDiff extension methods.
    /// Warn: use <see cref="GetEntityMappings" /> to set up the mappings.
    /// </summary>
    internal class EntityMappingProvider : SettingProvider
    {
        internal static string GraphDiffSettingGroupName = "Abp.EntityFramework.GraphDiff.Mappings";

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return GetEntityMappings()
                .Select(entityMapping => new SettingDefinition(
                    string.Format("Mapping for {0}", entityMapping.Key.FullName),
                    entityMapping.Key.FullName,
                    customData: entityMapping.Value,
                    group: new SettingDefinitionGroup(GraphDiffSettingGroupName, new FixedLocalizableString(GraphDiffSettingGroupName)))
                ).ToArray();
        }

        public virtual Dictionary<Type, Expression<Func<IUpdateConfiguration<Type>, object>>> GetEntityMappings()
        {
            return new Dictionary<Type, Expression<Func<IUpdateConfiguration<Type>, object>>>();
        }
    }
}