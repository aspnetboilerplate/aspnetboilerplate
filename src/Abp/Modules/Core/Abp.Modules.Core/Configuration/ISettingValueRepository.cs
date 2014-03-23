using Abp.Domain.Repositories;

namespace Abp.Configuration
{
    /// <summary>
    /// Repository to manage setting records.
    /// </summary>
    public interface ISettingValueRepository : IRepository<SettingValue, long>
    {

    }
}
