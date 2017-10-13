using System.Linq;
using System.Threading.Tasks;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Tenants
{
    public class TenantManagerTests : AbpZeroTestBase
    {
        private readonly TenantManager _tenantManager;

        public TenantManagerTests()
        {
            _tenantManager = Resolve<TenantManager>();
        }

        [Fact]
        public async Task Should_Not_Insert_Duplicate_Features()
        {
            const int tenantId = 1;

            UsingDbContext(tenantId, (context) =>
            {
                context.FeatureSettings.Count(f => f.TenantId == tenantId).ShouldBe(0);
            });

            await _tenantManager.SetFeatureValueAsync(tenantId, AppFeatures.SimpleIntFeature, "1");

            UsingDbContext(tenantId, (context) =>
            {
                context.FeatureSettings.Count(f => f.TenantId == tenantId).ShouldBe(1);
            });

            await _tenantManager.SetFeatureValueAsync(tenantId, AppFeatures.SimpleIntFeature, "2");

            UsingDbContext(tenantId, (context) =>
            {
                context.FeatureSettings.Count(f => f.TenantId == tenantId).ShouldBe(1);
            });

            await _tenantManager.SetFeatureValueAsync(tenantId, AppFeatures.SimpleIntFeature, "0");

            UsingDbContext(tenantId, (context) =>
            {
                context.FeatureSettings.Count(f => f.TenantId == tenantId).ShouldBe(0);
            });
        }
    }
}
