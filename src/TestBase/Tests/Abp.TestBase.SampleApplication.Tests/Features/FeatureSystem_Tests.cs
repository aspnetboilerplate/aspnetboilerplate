using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.TestBase.SampleApplication.ContacLists;
using Castle.MicroKernel.Registration;
using NSubstitute;
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
            //Note: NullFeatureChecker is used as default, and it always returns true
            var contactListAppService = Resolve<IContactListAppService>();
            contactListAppService.Test(); //Should not throw exception
        }

        [Fact]
        public void Should_Not_Call_Method_With_Feature_If_Not_Enabled()
        {
            var featureChecker = Substitute.For<IFeatureChecker>();
            featureChecker.IsEnabledAsync(SampleFeatureProvider.Names.Contacts).Returns(Task.FromResult(false));

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureChecker>().UsingFactoryMethod(() => featureChecker).LifestyleSingleton()
                );

            var contactListAppService = Resolve<IContactListAppService>();
            Assert.Throws<AbpAuthorizationException>(() => contactListAppService.Test());
        }
    }
}
