using Abp.Dependency;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public interface IAbpActionResultWrapperFactory : ITransientDependency
    {
        IAbpActionResultWrapper CreateFor([NotNull] ResultExecutingContext actionResult);
    }
}