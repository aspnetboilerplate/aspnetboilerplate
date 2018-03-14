using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Abp.Localization;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class MultiLingualEntity_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private ISettingManager _settingManager;

        private readonly IShopRepository<Product> _productRepository;
        private readonly IShopRepository<ProductTranslation> _productTranslationRepository;

        public MultiLingualEntity_Tests()
        {
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            RegisterFakeSettingManager();

            _productRepository = Resolve<IShopRepository<Product>>();
            _productTranslationRepository = Resolve<IShopRepository<ProductTranslation>>();
        }

        private void RegisterFakeSettingManager()
        {
            _settingManager = Substitute.For<ISettingManager>();
            _settingManager.GetSettingValueAsync(LocalizationSettingNames.DefaultLanguage).Returns("en");
            LocalIocManager.IocContainer.Register(Component.For<ISettingManager>().Instance(_settingManager).IsDefault());
        }

        [Fact]
        public void Get_MultiLingualEntity_Translation_Test()
        {
            var translation = _productTranslationRepository.Get(1);
            translation.ShouldNotBeNull();
        }

        [Fact]
        public async Task Get_MultiLingualEntity_Translation_With_Fallback_Test()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en");

                var watch = await _productTranslationRepository.GetWithFallback<ProductTranslation, Product>(1);
                watch.ShouldNotBeNull();
                watch.Name.ShouldBe("Watch");
                watch.Language.ShouldBe("en");

                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr");

                watch = await _productTranslationRepository.GetWithFallback<ProductTranslation, Product>(1);
                watch.ShouldNotBeNull();
                watch.Name.ShouldBe("Saat");
                watch.Language.ShouldBe("tr");

                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr");

                var bike = await _productTranslationRepository.GetWithFallback<ProductTranslation, Product>(2);
                bike.ShouldNotBeNull();
                bike.Name.ShouldBe("Bike");
                bike.Language.ShouldBe("en");
            }
        }
    }
}
