using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class UserLoginRepository : CoreModuleEfRepositoryBase<UserLogin, long>, IUserLoginRepository
    {

    }
}