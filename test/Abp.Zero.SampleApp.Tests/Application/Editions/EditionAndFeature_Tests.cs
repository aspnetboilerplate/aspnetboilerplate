using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Configuration.Startup;
using Abp.UI;
using Abp.Zero.SampleApp.Editions;
using Abp.Zero.SampleApp.Features;
using Abp.Zero.SampleApp.MultiTenancy;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Application.Editions
{
    public class EditionAndFeature_Tests : SampleAppTestBase
    {
        private readonly EditionManager _editionManager;
        private readonly TenantManager _tenantManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public EditionAndFeature_Tests()
        {
            _multiTenancyConfig = Resolve<IMultiTenancyConfig>();
            _editionManager = Resolve<EditionManager>();
            _tenantManager = Resolve<TenantManager>();
            _featureChecker = Resolve<FeatureChecker>();
        }

        [Fact]
        public async Task Should_Create_Edition()
        {
            await _editionManager.CreateAsync(new Edition { Name = "Standard", DisplayName = "Standard Edition" });

            UsingDbContext(context =>
            {
                context.Editions.FirstOrDefault(e => e.Name == "Standard").ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Delete_Edition()
        {
            var standardEdition = await CreateEditionAsync("Standard");

            await _editionManager.DeleteAsync(standardEdition);

            UsingDbContext(context =>
            {
                context.Editions.FirstOrDefault(e => e.Name == "Standard").ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Complex_Test_Scenario()
        {
            var standardEdition = await CreateEditionAsync("Standard");
            var defaultTenant = GetDefaultTenant();

            defaultTenant.EditionId = standardEdition.Id;
            await _tenantManager.UpdateAsync(defaultTenant);

            AbpSession.TenantId = defaultTenant.Id;

            //No value initially
            (await _editionManager.GetFeatureValueOrNullAsync(standardEdition.Id, AppFeatureProvider.MyBoolFeature)).ShouldBeNull();
            (await _editionManager.GetFeatureValueOrNullAsync(standardEdition.Id, AppFeatureProvider.MyNumericFeature)).ShouldBeNull();
            (await _tenantManager.GetFeatureValueOrNullAsync(defaultTenant.Id, AppFeatureProvider.MyBoolFeature)).ShouldBeNull();
            (await _tenantManager.GetFeatureValueOrNullAsync(defaultTenant.Id, AppFeatureProvider.MyNumericFeature)).ShouldBeNull();

            //Should get default values
            (await _featureChecker.IsEnabledAsync(AppFeatureProvider.MyBoolFeature)).ShouldBeFalse();
            (await _featureChecker.GetValueAsync(AppFeatureProvider.MyNumericFeature)).ShouldBe("42");

            //Set edition values
            await _editionManager.SetFeatureValueAsync(standardEdition.Id, AppFeatureProvider.MyBoolFeature, "true");
            await _editionManager.SetFeatureValueAsync(standardEdition.Id, AppFeatureProvider.MyNumericFeature, "43");

            //Should get new values for edition
            (await _editionManager.GetFeatureValueOrNullAsync(standardEdition.Id, AppFeatureProvider.MyNumericFeature)).ShouldBe("43");
            (await _editionManager.GetFeatureValueOrNullAsync(standardEdition.Id, AppFeatureProvider.MyBoolFeature)).ShouldBe("true");

            //Should get edition values for tenant
            (await _featureChecker.GetValueAsync(AppFeatureProvider.MyNumericFeature)).ShouldBe("43");
            (await _featureChecker.IsEnabledAsync(AppFeatureProvider.MyBoolFeature)).ShouldBeTrue();
        }

        [Fact]
        public async Task SetFeatureValue_Value_Should_Valid()
        {
            var standardEdition = await CreateEditionAsync("Standard");
            var defaultTenant = GetDefaultTenant();

            defaultTenant.EditionId = standardEdition.Id;
            await _tenantManager.UpdateAsync(defaultTenant);

            AbpSession.TenantId = defaultTenant.Id;

            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _editionManager.SetFeatureValueAsync(standardEdition.Id, AppFeatureProvider.MyNumericFeature,
                    "101")); // 101 not valid
        }

        [Fact]
        public async Task Should_Ignore_Feature_Check_For_Host_Users()
        {
            var standardEdition = await CreateEditionAsync("Standard");
            var defaultTenant = GetDefaultTenant();

            defaultTenant.EditionId = standardEdition.Id;
            await _tenantManager.UpdateAsync(defaultTenant);

            //Set edition values
            await _editionManager.SetFeatureValueAsync(standardEdition.Id, AppFeatureProvider.MyBoolFeature, "true");

            _multiTenancyConfig.IgnoreFeatureCheckForHostUsers = true;
            AbpSession.TenantId = null;
            (await _featureChecker.IsEnabledAsync(AppFeatureProvider.MyBoolFeature)).ShouldBeTrue();
        }

        private async Task<Edition> CreateEditionAsync(string name)
        {
            UsingDbContext(context => { context.Editions.Add(new Edition { Name = name, DisplayName = name + " Edition" }); });

            var standardEdition = await _editionManager.FindByNameAsync("Standard");
            standardEdition.ShouldNotBeNull();

            return standardEdition;
        }
    }
}
