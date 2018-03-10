using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class MultiLingualEntity_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IShopRepository<Product> _productRepository;
        private readonly IShopRepository<ProductTranslation> _productTranslationRepository;

        public MultiLingualEntity_Tests()
        {
            _productRepository = Resolve<IShopRepository<Product>>();
            _productTranslationRepository = Resolve<IShopRepository<ProductTranslation>>();
        }

        [Fact]
        public void Get_MultiLingualEntity_Translation_Test()
        {
            var translation = _productTranslationRepository.Get(1);
            translation.ShouldNotBeNull();
        }
    }
}
