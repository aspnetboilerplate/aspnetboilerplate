using Abp.Localization;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class ApplicationLanguageMap : EntityMap<ApplicationLanguage>
    {
        public ApplicationLanguageMap()
            : base("AbpLanguages")
        {
            Map(x => x.TenantId);
            Map(x => x.Name);
            Map(x => x.DisplayName);
            Map(x => x.Icon);
            
            this.MapFullAudited();
        }
    }
}