using Abp.Configuration;
using Abp.Domain.Repositories.NHibernate;

namespace Abp.Zero.Repositories.NHibernate
{
    public class SettingRepository : NhRepositoryBase<Setting, long>, ISettingRepository
    {

    }
}