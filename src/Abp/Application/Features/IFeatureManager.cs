using System.Collections.Generic;

namespace Abp.Application.Features
{
    public interface IFeatureManager
    {
        Feature Get(string name);

        Feature GetOrNull(string name);

        IReadOnlyList<Feature> GetAll();
    }
}