using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public interface IAbpActionResultWrapperFactory
    {
        IAbpActionResultWrapper CreateFor([NotNull] ResultExecutingContext actionResult);
    }
}