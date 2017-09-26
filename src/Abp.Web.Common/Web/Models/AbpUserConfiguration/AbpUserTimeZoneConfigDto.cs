namespace Abp.Web.Models.AbpUserConfiguration
{
    public class AbpUserTimeZoneConfigDto
    {
        public AbpUserWindowsTimeZoneConfigDto Windows { get; set; }

        public AbpUserIanaTimeZoneConfigDto Iana { get; set; }
    }
}