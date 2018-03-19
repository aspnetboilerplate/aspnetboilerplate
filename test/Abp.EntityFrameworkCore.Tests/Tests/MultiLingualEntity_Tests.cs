using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Abp.Localization;
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
            var translation = _productTranslationRepository.FirstOrDefault(e => e.CoreId == 2 && e.Language == "fr");
            translation.ShouldNotBeNull();
            translation.Language.ShouldBe("fr");
            translation.Name.ShouldBe("Bicyclette");
        }

        [Fact]
        public void Get_MultiLingualEntity_With_Translations_Test()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var product = _productRepository.GetAllIncluding(p => p.Translations).FirstOrDefault(p => p.Id == 1);
                product.ShouldNotBeNull();
                product.Translations.Count.ShouldBe(2);
            }
        }
    }
}
