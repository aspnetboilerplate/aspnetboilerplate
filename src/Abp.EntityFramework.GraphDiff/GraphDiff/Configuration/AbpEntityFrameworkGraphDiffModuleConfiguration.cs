using System.Collections.Generic;
using Abp.GraphDiff.Mapping;

namespace Abp.GraphDiff.Configuration
{
    public class AbpEntityFrameworkGraphDiffModuleConfiguration : IAbpEntityFrameworkGraphDiffModuleConfiguration
    {
        public List<EntityMapping> EntityMappings { get; set; }
    }
}