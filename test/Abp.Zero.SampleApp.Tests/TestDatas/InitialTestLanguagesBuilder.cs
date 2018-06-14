using System.Linq;
using Abp.Localization;
using Abp.Zero.SampleApp.EntityFramework;
using Abp.Zero.SampleApp.MultiTenancy;

namespace Abp.Zero.SampleApp.Tests.TestDatas
{
    public class InitialTestLanguagesBuilder
    {
        private readonly AppDbContext _context;

        public InitialTestLanguagesBuilder(AppDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            InitializeLanguagesOnDatabase();
        }

        private void InitializeLanguagesOnDatabase()
        {
            var defaultTenant = _context.Tenants.Single(t => t.TenancyName == Tenant.DefaultTenantName);
            
            //Host languages
            _context.Languages.Add(new ApplicationLanguage { Name = "en", DisplayName = "English" });
            _context.Languages.Add(new ApplicationLanguage { Name = "tr", DisplayName = "Türkçe" });
            _context.Languages.Add(new ApplicationLanguage { Name = "de", DisplayName = "German" });

            //Default tenant languages
            _context.Languages.Add(new ApplicationLanguage { Name = "zh-Hans", DisplayName = "简体中文", TenantId = defaultTenant.Id });
        }
    }
}