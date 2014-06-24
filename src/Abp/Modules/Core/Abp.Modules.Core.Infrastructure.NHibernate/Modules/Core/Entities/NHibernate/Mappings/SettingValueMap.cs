using Abp.Configuration;
using Abp.Domain.Entities.Mapping;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class SettingValueMap : EntityMap<Setting, long>
    {
        public SettingValueMap()
            : base("AbpSettings")
        {
            Map(x => x.TenantId);
            Map(x => x.UserId);
            Map(x => x.Name);
            Map(x => x.Value);

            this.MapAudited();
        }
    }
}