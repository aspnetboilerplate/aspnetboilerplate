using Abp.Domain.Repositories;

namespace Abp.Configuration
{
    /// <summary>
    /// Repository to manage setting records.
    /// </summary>
    public interface ISettingRepository : IRepository<Setting, long>
    {

    }
}
