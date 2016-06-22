using Abp.Dependency;

namespace Abp.AspNetCore
{
    public class AbpServiceOptions
    {
        public IIocManager IocManager { get; set; }
    }
}