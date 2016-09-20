using System.Collections.Generic;
using System.Web.Http;
using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.WebApi.Configuration
{
    internal class AbpWebApiConfiguration : IAbpWebApiConfiguration
    {
        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        public WrapResultAttribute DefaultWrapResultAttribute { get; }

        public WrapResultAttribute DefaultDynamicApiWrapResultAttribute { get; }

        public List<string> ResultWrappingIgnoreUrls { get; }

        public HttpConfiguration HttpConfiguration { get; set; }

        public bool IsValidationEnabledForControllers { get; set; }

        public bool IsAutomaticAntiForgeryValidationEnabled { get; set; }

        public bool SetNoCacheForAllResponses { get; set; }

        public AbpWebApiConfiguration()
        {
            HttpConfiguration = GlobalConfiguration.Configuration;
            DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            DefaultWrapResultAttribute = new WrapResultAttribute(false);
            DefaultDynamicApiWrapResultAttribute = new WrapResultAttribute();
            ResultWrappingIgnoreUrls = new List<string>();
            IsValidationEnabledForControllers = true;
            IsAutomaticAntiForgeryValidationEnabled = true;
            SetNoCacheForAllResponses = true;
        }
    }
}