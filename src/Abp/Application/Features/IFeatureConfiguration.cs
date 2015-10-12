using Abp.Collections;

namespace Abp.Application.Features
{
    public interface IFeatureConfiguration
    {
        ITypeList<FeatureProvider> Providers { get; }
    }
}