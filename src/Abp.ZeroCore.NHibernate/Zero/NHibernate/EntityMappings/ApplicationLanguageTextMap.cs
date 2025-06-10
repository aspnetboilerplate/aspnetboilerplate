using Abp.Localization;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class ApplicationLanguageTextMap : EntityMap<ApplicationLanguageText, long>
{
    public ApplicationLanguageTextMap()
        : base("AbpLanguageTexts")
    {
        Map(x => x.Key)
            .Length(ApplicationLanguageText.MaxKeyLength)
            .Not.Nullable();
        Map(x => x.LanguageName)
            .Length(ApplicationLanguageText.MaxSourceNameLength)
            .Not.Nullable();
        Map(x => x.Source)
            .Length(ApplicationLanguageText.MaxSourceNameLength)
            .Not.Nullable();
        Map(x => x.TenantId);
        Map(x => x.Value)
            .Length(ApplicationLanguageText.MaxValueLength)
            .Not.Nullable();

        this.MapAudited();
    }
}