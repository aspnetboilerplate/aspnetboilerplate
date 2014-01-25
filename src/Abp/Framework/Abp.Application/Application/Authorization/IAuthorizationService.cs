using Abp.Dependency;
using Abp.Domain.Services;

namespace Abp.Application.Authorization
{
    public interface IAuthorizationService : ITransientDependency
    {
        bool HasAnyOfPermissions(string[] permissionNames);
        bool HasAllOfPermissions(string[] permissionNames);

        bool HasPermission(string permissionName);
    }
}