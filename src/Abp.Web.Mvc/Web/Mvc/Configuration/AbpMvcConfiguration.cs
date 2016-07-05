using Abp.Web.Models;

namespace Abp.Web.Mvc.Configuration
{
    public class AbpMvcConfiguration : IAbpMvcConfiguration
    {
        public WrapResultAttribute DefaultWrapResultAttribute { get; set; }

        public AbpMvcConfiguration()
        {
            DefaultWrapResultAttribute = new WrapResultAttribute();
        }
    }
}