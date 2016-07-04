using Abp.Web.Models;

namespace Abp.Web.Mvc.Configuration
{
    public interface IAbpMvcConfiguration
    {
        WrapResultAttribute DefaultWrapResultAttribute { get; set; }
    }
}
