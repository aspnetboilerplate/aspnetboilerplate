using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Tenants
{
    public class TenantManagerTests : AbpZeroTestBase
    {
        private readonly TenantManager _tenantManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TenantManagerTests()
        {
            _tenantManager = Resolve<TenantManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Should_Not_Insert_Duplicate_Features()
        {
            const int tenantId = 1;

            UsingDbContext(tenantId, (context) =>
            {
                context.FeatureSettings.Count(f => f.TenantId == tenantId).ShouldBe(0);
            });

            await ChangeTenantFeatureValueAsync(tenantId, AppFeatures.SimpleIntFeature, "1");

            UsingDbContext(tenantId, (context) =>
            {
                context.FeatureSettings.Count(f => f.TenantId == tenantId).ShouldBe(1);
            });

            await ChangeTenantFeatureValueAsync(tenantId, AppFeatures.SimpleIntFeature, "2");

            UsingDbContext(tenantId, (context) =>
            {
                context.FeatureSettings.Count(f => f.TenantId == tenantId).ShouldBe(1);
            });

            await ChangeTenantFeatureValueAsync(tenantId, AppFeatures.SimpleIntFeature, "0");

            UsingDbContext(tenantId, (context) =>
            {
                context.FeatureSettings.Count(f => f.TenantId == tenantId).ShouldBe(0);
            });
        }

        private async Task ChangeTenantFeatureValueAsync(int tenantId, string name, string value)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    await _tenantManager.SetFeatureValueAsync(tenantId, name, value);
                }

                await uow.CompleteAsync();
            }
        }
    }
}
