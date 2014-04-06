using Abp.Configuration;
using Abp.Domain.Repositories.EntityFramework;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class SettingRepository : EfRepositoryBase<Setting, long>, ISettingRepository
    {

    }
}