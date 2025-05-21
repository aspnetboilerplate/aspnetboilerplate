using Abp.Localization;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class ApplicationLanguageMap : EntityMap<ApplicationLanguage>
{
    public ApplicationLanguageMap()
        : base("AbpLanguages")
    {
        Map(x => x.TenantId)
            .Nullable();
        Map(x => x.Name)
            .Length(ApplicationLanguage.MaxNameLength)
            .Not.Nullable();
        Map(x => x.DisplayName)
            .Length(ApplicationLanguage.MaxDisplayNameLength)
            .Not.Nullable();
        Map(x => x.Icon)
            .Length(ApplicationLanguage.MaxIconLength);
        Map(x => x.IsDisabled)
            .Not.Nullable();

        this.MapFullAudited();
    }
}