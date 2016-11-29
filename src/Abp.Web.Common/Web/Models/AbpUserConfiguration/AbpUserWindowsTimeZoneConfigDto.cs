namespace Abp.Web.Models.AbpUserConfiguration
{
    public class AbpUserWindowsTimeZoneConfigDto
    {
        public string TimeZoneId { get; set; }

        public double BaseUtcOffsetInMilliseconds { get; set; }

        public double CurrentUtcOffsetInMilliseconds { get; set; }

        public bool IsDaylightSavingTimeNow { get; set; }
    }
}