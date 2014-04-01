using Abp.Configuration;
using Abp.Domain.Repositories.EntityFramework;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public class SettingValueRepository : EfRepositoryBase<SettingValue, long>, ISettingValueRepository
    {

    }
}