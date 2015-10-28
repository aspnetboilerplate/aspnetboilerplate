using System.Threading.Tasks;
using Abp.Application.Services;
using MyAbpZeroProject.Roles.Dto;

namespace MyAbpZeroProject.Roles
{
    public interface IRoleAppService : IApplicationService
    {
        Task UpdateRolePermissions(UpdateRolePermissionsInput input);
    }
}
