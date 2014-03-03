using Abp.Configuration;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class SettingValueRecordMap : EntityMap<SettingValueRecord, long>
    {
        public SettingValueRecordMap()
            : base("AbpSettings")
        {
            Map(x => x.UserId);
            Map(x => x.Name);
            Map(x => x.Value);
        }
    }
}