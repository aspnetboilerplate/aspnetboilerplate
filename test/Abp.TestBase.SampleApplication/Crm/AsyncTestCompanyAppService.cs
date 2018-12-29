using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;

namespace Abp.TestBase.SampleApplication.Crm
{
    [AbpAuthorize("GetCompanyPermission")]
    public class AsyncTestCompanyAppService : AsyncCrudAppService<Company, CompanyDto>
    {
        public AsyncTestCompanyAppService(IRepository<Company> repository)
            : base(repository)
        {
            CreatePermissionName = "CreateCompanyPermission";
        }
    }
}