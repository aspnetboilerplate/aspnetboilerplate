namespace Abp.Web.Models.AbpUserConfiguration
{
    public class AbpUserAntiForgeryConfigDto
    {
        public string TokenCookieName { get; set; }

        public string TokenHeaderName { get; set; }
    }
}