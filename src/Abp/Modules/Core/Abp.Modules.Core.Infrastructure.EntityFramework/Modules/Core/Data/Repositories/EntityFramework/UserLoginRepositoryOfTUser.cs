using Abp.Domain.Repositories.EntityFramework;
using Abp.Security.Users;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class UserLoginRepository : EfRepositoryBase<UserLogin, long>, IUserLoginRepository
    {

    }
}