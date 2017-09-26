using Abp.Application.Features;
using Abp.MultiTenancy;
using Abp.Zero.SampleApp.Features;
using Abp.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.MultiTenancy
{
    public class TenantFeature_Tests : SampleAppTestBase
    {
        private readonly TenantManager _tenantManager;
        private readonly IFeatureChecker _featureChecker;

        public TenantFeature_Tests()
        {
            _tenantManager = Resolve<TenantManager>();
            _featureChecker = Resolve<IFeatureChecker>();
        }

        [Fact]
        public void Changing_Tenant_Feature_Should_Not_Effect_Other_Tenants()
        {
            //Create tenants
            var firstTenantId = UsingDbContext(context =>
            {
                var firstTenant = new Tenant("Tenant1", "Tenant1");
                context.Tenants.Add(firstTenant);
                context.SaveChanges();
                return firstTenant.Id;
            });

            var secondTenantId = UsingDbContext(context =>
            {
                var secondTenant = new Tenant("Tenant2", "Tenant2");
                context.Tenants.Add(secondTenant);
                context.SaveChanges();
                return secondTenant.Id;
            });
            
            _tenantManager.SetFeatureValue(firstTenantId, AppFeatureProvider.MyBoolFeature, "true");
            _featureChecker.IsEnabled(secondTenantId, AppFeatureProvider.MyBoolFeature).ShouldBe(false);
        }
    }
}
