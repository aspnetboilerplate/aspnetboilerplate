using System.Collections.Generic;
using Abp.GraphDiff.Mapping;

namespace Abp.GraphDiff.Configuration.Startup
{
    public interface IAbpEntityFrameworkGraphDiffModuleConfiguration
    {
        List<EntityMapping> EntityMappings { get; set; }
    }
}
