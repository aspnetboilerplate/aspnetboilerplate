using Abp.Configuration;
using Abp.Domain.Repositories.NHibernate;

namespace Abp.Modules.Core.Data.Repositories.NHibernate
{
    public class SettingValueRepository : NhRepositoryBase<Setting, long>, ISettingValueRepository
    {

    }
}