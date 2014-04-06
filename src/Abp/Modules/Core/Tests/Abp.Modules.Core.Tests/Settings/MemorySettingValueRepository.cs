using Abp.Configuration;

namespace Abp.Modules.Core.Tests.Settings
{
    public class MemorySettingValueRepository : MemoryBasedRepository<Setting, long>, ISettingValueRepository
    {
        public MemorySettingValueRepository(IPrimaryKeyGenerator<long> primaryKeyGenerator) 
            : base(primaryKeyGenerator)
        {
        }
    }
}