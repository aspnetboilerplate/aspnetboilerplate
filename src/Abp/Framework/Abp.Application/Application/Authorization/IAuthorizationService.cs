using Abp.Dependency;

namespace Abp.Application.Authorization
{
    public interface IAuthorizationService : ITransientDependency
    {
        bool HasAnyOfPermissions(string[] permissionNames);

        bool HasAllOfPermissions(string[] permissionNames);
    }
}