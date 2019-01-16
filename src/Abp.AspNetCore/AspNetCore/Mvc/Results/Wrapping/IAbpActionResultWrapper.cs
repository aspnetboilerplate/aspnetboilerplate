using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public interface IAbpActionResultWrapper
    {
        void Wrap(ResultExecutingContext actionResult, WrapResultAttribute wrapResultAttribute);
    }
}