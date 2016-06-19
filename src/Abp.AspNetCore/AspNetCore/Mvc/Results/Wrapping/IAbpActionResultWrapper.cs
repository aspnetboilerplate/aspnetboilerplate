using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public interface IAbpActionResultWrapper
    {
        void Wrap(IActionResult actionResult);
    }
}