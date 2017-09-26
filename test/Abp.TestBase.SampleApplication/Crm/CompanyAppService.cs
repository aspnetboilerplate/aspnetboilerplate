using Abp.Application.Services;
using Abp.Domain.Repositories;

namespace Abp.TestBase.SampleApplication.Crm
{
    public class CompanyAppService : CrudAppService<Company, CompanyDto, int>
    {
        public CompanyAppService(IRepository<Company, int> repository) : base(repository)
        {
            GetPermissionName = "GetCompanyPermission";
            GetAllPermissionName = "GetAllCompaniesPermission";
            CreatePermissionName = "CreateCompanyPermission";
            UpdatePermissionName = "UpdateCompanyPermission";
            DeletePermissionName = "DeleteCompanyPermission";
        }
    }
}
