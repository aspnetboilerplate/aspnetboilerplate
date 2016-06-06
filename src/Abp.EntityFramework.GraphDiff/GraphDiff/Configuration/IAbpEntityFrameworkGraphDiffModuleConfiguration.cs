using System.Collections.Generic;
using Abp.EntityFramework.GraphDiff.Mapping;

namespace Abp.EntityFramework.GraphDiff.Configuration
{
    public interface IAbpEntityFrameworkGraphDiffModuleConfiguration
    {
        List<EntityMapping> EntityMappings { get; set; }
    }
}