using Abp.Configuration;

namespace Abp.Modules.Core.Tests.Settings
{
    public class MemorySettingRepository : MemoryBasedRepository<Setting, long>, ISettingRepository
    {
        public MemorySettingRepository(IPrimaryKeyGenerator<long> primaryKeyGenerator) 
            : base(primaryKeyGenerator)
        {
        }
    }
}