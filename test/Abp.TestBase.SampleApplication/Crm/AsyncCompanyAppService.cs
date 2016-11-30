using Abp.Application.Services;
using Abp.Domain.Repositories;

namespace Abp.TestBase.SampleApplication.Crm
{
    public class AsyncCompanyAppService : AsyncCrudAppService<Company, CompanyDto>
    {
        public AsyncCompanyAppService(IRepository<Company> repository)
            : base(repository)
        {
            GetPermissionName = "GetCompanyPermission";
            GetAllPermissionName = "GetAllCompaniesPermission";
            CreatePermissionName = "CreateCompanyPermission";
            UpdatePermissionName = "UpdateCompanyPermission";
            DeletePermissionName = "DeleteCompanyPermission";
        }
    }
}