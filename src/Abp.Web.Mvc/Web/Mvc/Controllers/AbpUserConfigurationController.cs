using System.Threading.Tasks;
using Abp.Web.Configuration;
using Abp.Web.Models.AbpUserConfiguration;

namespace Abp.Web.Mvc.Controllers
{
    public class AbpUserConfigurationController : AbpController
    {
        private readonly AbpUserConfigurationBuilder _abpUserConfigurationBuilder;

        public AbpUserConfigurationController(AbpUserConfigurationBuilder abpUserConfigurationBuilder)
        {
            _abpUserConfigurationBuilder = abpUserConfigurationBuilder;
        }

        public async Task<AbpUserConfigurationDto> GetAll()
        {
            var userConfig = await _abpUserConfigurationBuilder.GetAll();
            return userConfig;
        }
    }
}