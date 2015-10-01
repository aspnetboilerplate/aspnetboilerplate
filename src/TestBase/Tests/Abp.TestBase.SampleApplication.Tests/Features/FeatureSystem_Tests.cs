using Abp.Application.Features;
using Abp.TestBase.SampleApplication.ContacLists;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Features
{
    public class FeatureSystem_Tests: SampleApplicationTestBase
    {
        private readonly IFeatureManager _featureManager;

        public FeatureSystem_Tests()
        {
            _featureManager = Resolve<IFeatureManager>();
        }

        [Fact]
        public void Should_Get_Defined_Features()
        {
            _featureManager.Get(SampleFeatureProvider.Names.Contacts).ShouldNotBe(null);
            _featureManager.GetAll().Count.ShouldBe(1);
        }

        [Fact]
        public void Should_Not_Get_Undefined_Features()
        {
            _featureManager.GetOrNull("NonExistingFeature").ShouldBe(null);
            Assert.Throws<AbpException>(() => _featureManager.Get("NonExistingFeature"));
        }

        [Fact]
        public void Should_Call_Method_With_Feature_If_Enabled()
        {
            var contactListAppService = Resolve<IContactListAppService>();
            contactListAppService.Test(); //Should not throw exception
        }
    }
}
