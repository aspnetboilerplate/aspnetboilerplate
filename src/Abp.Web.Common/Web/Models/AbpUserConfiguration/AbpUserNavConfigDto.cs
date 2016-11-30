using System.Collections.Generic;
using Abp.Application.Navigation;

namespace Abp.Web.Models.AbpUserConfiguration
{
    public class AbpUserNavConfigDto
    {
        public Dictionary<string, UserMenu> Menus { get; set; }
    }
}