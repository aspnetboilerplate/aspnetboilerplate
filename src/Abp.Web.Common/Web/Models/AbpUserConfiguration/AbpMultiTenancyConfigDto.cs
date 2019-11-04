namespace Abp.Web.Models.AbpUserConfiguration
{
    public class AbpMultiTenancyConfigDto
    {
        public bool IsEnabled { get; set; }

        public bool IgnoreFeatureCheckForHostUsers { get; set; }

        public AbpMultiTenancySidesConfigDto Sides { get; private set; }

        public AbpMultiTenancyConfigDto()
        {
            Sides = new AbpMultiTenancySidesConfigDto();
        }
    }
}