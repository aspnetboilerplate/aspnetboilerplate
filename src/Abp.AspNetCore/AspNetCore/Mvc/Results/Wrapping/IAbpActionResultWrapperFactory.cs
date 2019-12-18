using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public interface IAbpActionResultWrapperFactory : ITransientDependency
    {
        IAbpActionResultWrapper CreateFor(FilterContext context);
    }
}