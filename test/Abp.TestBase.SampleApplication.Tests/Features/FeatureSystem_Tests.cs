using System.Globalization;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.TestBase.SampleApplication.ContactLists;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Features
{
    public class FeatureSystem_Tests : SampleApplicationTestBase
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
            _featureManager.Get(SampleFeatureProvider.Names.MaxContactCount).ShouldNotBe(null);
            _featureManager.GetAll().Count.ShouldBe(3);
        }

        [Fact]
        public void Should_Not_Get_Undefined_Features()
        {
            _featureManager.GetOrNull("NonExistingFeature").ShouldBe(null);
            Assert.Throws<AbpException>(() => _featureManager.Get("NonExistingFeature"));
        }

        [Fact]
        public virtual void Should_Get_Feature_Values()
        {
            var featureValueStore = Substitute.For<IFeatureValueStore>();
            featureValueStore.GetValueOrNullAsync(1, _featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns(Task.FromResult("true"));
            featureValueStore.GetValueOrNullAsync(1, _featureManager.Get(SampleFeatureProvider.Names.MaxContactCount)).Returns(Task.FromResult("20"));
            featureValueStore.GetValueOrNull(1, _featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns("true");
            featureValueStore.GetValueOrNull(1, _featureManager.Get(SampleFeatureProvider.Names.MaxContactCount)).Returns("20");

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureValueStore>().Instance(featureValueStore).LifestyleSingleton()
                );

            var featureChecker = Resolve<IFeatureChecker>();
            featureChecker.GetValue(SampleFeatureProvider.Names.Contacts).To<bool>().ShouldBeTrue();
            featureChecker.IsEnabled(SampleFeatureProvider.Names.Contacts).ShouldBeTrue();
            featureChecker.GetValue(SampleFeatureProvider.Names.MaxContactCount).To<int>().ShouldBe(20);
        }

        [Fact]
        public void Should_Call_Method_With_Feature_If_Enabled()
        {
            var featureValueStore = Substitute.For<IFeatureValueStore>();
            featureValueStore.GetValueOrNullAsync(1, _featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns(Task.FromResult("true"));
            featureValueStore.GetValueOrNull(1, _featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns("true");

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureValueStore>().Instance(featureValueStore).LifestyleSingleton()
                );

            var contactListAppService = Resolve<IContactListAppService>();
            contactListAppService.Test(); //Should not throw exception
        }

        [Fact]
        public void Should_Not_Call_Method_With_Feature_If_Not_Enabled()
        {
            var featureValueStore = Substitute.For<IFeatureValueStore>();
            featureValueStore.GetValueOrNullAsync(1, _featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns(Task.FromResult("false"));
            featureValueStore.GetValueOrNullAsync(1, _featureManager.Get(SampleFeatureProvider.Names.MaxContactCount)).Returns(Task.FromResult("20"));

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureValueStore>().Instance(featureValueStore).LifestyleSingleton()
                );

            var contactListAppService = Resolve<IContactListAppService>();
            Assert.Throws<AbpAuthorizationException>(() => contactListAppService.Test());
        }


        [Fact]
        public void Feature_Checker_Exception_Should_Use_Localized_DisplayName()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en");

            var featureValueStore = Substitute.For<IFeatureValueStore>();
            featureValueStore.GetValueOrNullAsync(1, _featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns(Task.FromResult("false"));

            var contactListAppService = Resolve<IContactListAppService>();
            var ex = Assert.Throws<AbpAuthorizationException>(() => contactListAppService.Test());
            ex.Message.ShouldContain("My Contacts");
        }


        [Fact]
        public void Should_Override_Child_Feature()
        {
            var childFeature = _featureManager.Get(SampleFeatureProvider.Names.ChildFeatureToOverride);
            childFeature.ShouldNotBeNull();
            childFeature.DefaultValue.ShouldBe("ChildFeatureToOverride");
        }

        [Fact]
        public void Should_Remove_Child_Feature()
        {
            Should.Throw<AbpException>(() => {
                var childFeature = _featureManager.Get(SampleFeatureProvider.Names.ChildFeatureToDelete);
            });
        }
    }
}
