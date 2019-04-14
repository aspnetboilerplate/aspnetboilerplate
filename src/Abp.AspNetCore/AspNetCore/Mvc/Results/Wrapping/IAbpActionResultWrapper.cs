using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public interface IAbpActionResultWrapper
    {
        void Wrap(FilterContext context);
    }
}