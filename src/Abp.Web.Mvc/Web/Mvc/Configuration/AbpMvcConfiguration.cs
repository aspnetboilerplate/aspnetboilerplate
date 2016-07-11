using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Configuration
{
    public class AbpMvcConfiguration : IAbpMvcConfiguration
    {
        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        public WrapResultAttribute DefaultWrapResultAttribute { get; }

        public AbpMvcConfiguration()
        {
            DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            DefaultWrapResultAttribute = new WrapResultAttribute();
        }
    }
}