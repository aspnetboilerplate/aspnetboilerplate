using System.Globalization;
using System.Linq;
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

        [Fact]
        public async Task Get_MultiLingualEntity_Including_Translation_Test()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en");

                var watch = await _productRepository.GetWithTranslation<Product, ProductTranslation>(1);
                watch.ShouldNotBeNull();
                watch.Translations.Count.ShouldBe(1);
                watch.Translations.Single(t => t.Language == "en").ShouldNotBeNull();
                watch.Translations.Single(t => t.Language == "en").Name.ShouldBe("Watch");

                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr");

                watch = await _productRepository.GetWithTranslation<Product, ProductTranslation>(1);
                watch.Translations.Count.ShouldBe(2);
                watch.Translations.Single(t => t.Language == "en").ShouldNotBeNull();
                watch.Translations.Single(t => t.Language == "en").Name.ShouldBe("Watch");
                watch.Translations.Single(t => t.Language == "tr").ShouldNotBeNull();
                watch.Translations.Single(t => t.Language == "tr").Name.ShouldBe("Saat");

                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr");

                var bike = await _productRepository.GetWithTranslation<Product, ProductTranslation>(2);
                bike.Translations.Count.ShouldBe(1);
                bike.Translations.Single(t => t.Language == "en").ShouldNotBeNull();
                bike.Translations.Single(t => t.Language == "en").Name.ShouldBe("Bike");
            }
        }

        [Fact]
        public async Task GetAll_MultiLingualEntity_Including_Translation_Test()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en");

                var products = (await _productRepository.GetAllIncludingTranslation<Product, ProductTranslation>())
                    .ToList();
                
                products.Count.ShouldBe(2);
                products[0].Translations.Count.ShouldBe(1);
                products[1].Translations.Count.ShouldBe(1);

                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr");

                products = (await _productRepository.GetAllIncludingTranslation<Product, ProductTranslation>())
                    .ToList();

                products.Count.ShouldBe(2);
                products[0].Translations.Count.ShouldBe(2);
                products[1].Translations.Count.ShouldBe(1);
            }
        }
    }
}
