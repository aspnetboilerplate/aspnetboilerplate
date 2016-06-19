using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Results.Wrapping
{
    public class NullAbpActionResultWrapper : IAbpActionResultWrapper
    {
        public void Wrap(IActionResult actionResult)
        {
            
        }
    }
}