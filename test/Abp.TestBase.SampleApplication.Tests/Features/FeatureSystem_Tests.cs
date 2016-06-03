using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Extensions;
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
            _featureManager.Get(SampleFeatureProvider.Names.MaxContactCount).ShouldNotBe(null);
            _featureManager.GetAll().Count.ShouldBe(2);
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

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureValueStore>().UsingFactoryMethod(() => featureValueStore).LifestyleSingleton()
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

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureValueStore>().UsingFactoryMethod(() => featureValueStore).LifestyleSingleton()
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
                Component.For<IFeatureValueStore>().UsingFactoryMethod(() => featureValueStore).LifestyleSingleton()
                );

            var contactListAppService = Resolve<IContactListAppService>();
            Assert.Throws<AbpAuthorizationException>(() => contactListAppService.Test());
        }
    }
}
