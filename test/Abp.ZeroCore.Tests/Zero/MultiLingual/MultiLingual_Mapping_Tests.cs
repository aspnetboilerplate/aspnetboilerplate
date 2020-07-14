using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.ZeroCore.SampleApp.Application.Shop;
using Shouldly;
using Xunit;

namespace Abp.Zero.MultiLingual
{
    public class MultiLingual_Mapping_Tests : AbpZeroTestBase
    {
        private readonly IProductAppService _productAppService;
        private readonly IOrderAppService _orderAppService;

        public MultiLingual_Mapping_Tests()
        {
            _productAppService = Resolve<IProductAppService>();
            _orderAppService = Resolve<IOrderAppService>();
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
        }

        [Fact]
        public async Task CreateMultiLingualMap_Tests()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("tr");

            var products = await _productAppService.GetProducts();
            products.ShouldNotBeNull();

            products.Items.Count.ShouldBe(3);
            var product1 = products.Items[0];
            var product2 = products.Items[1];
            var product3 = products.Items[2];

            product1.Language.ShouldBe("tr");
            product1.Name.ShouldBe("Saat");

            product2.Language.ShouldBe("en");
            product2.Name.ShouldBe("Bike");

            product3.Language.ShouldBe("it");
            product3.Name.ShouldBe("Giornale");

            CultureInfo.CurrentUICulture = new CultureInfo("fr");

            products = await _productAppService.GetProducts();
            products.ShouldNotBeNull();

            products.Items.Count.ShouldBe(3);
            product1 = products.Items[0];
            product2 = products.Items[1];
            product3 = products.Items[2];

            product1.Language.ShouldBe("en");
            product1.Name.ShouldBe("Watch");

            product2.Language.ShouldBe("fr");
            product2.Name.ShouldBe("Bicyclette");

            product3.Language.ShouldBe("it");
            product3.Name.ShouldBe("Giornale");
        }

        [Fact]
        public async Task CreateMultiLingualMap_RegionSpecificLanguage_ShouldFallbackToGlobalLanguage()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-GB");

            var products = await _productAppService.GetProducts();
            products.ShouldNotBeNull();

            products.Items.Count.ShouldBe(3);
            var product1 = products.Items[0];
            var product2 = products.Items[1];
            var product3 = products.Items[2];

            product1.Language.ShouldBe("en");
            product1.Name.ShouldBe("Watch");

            product2.Language.ShouldBe("en");
            product2.Name.ShouldBe("Bike");

            product3.Language.ShouldBe("it");
            product3.Name.ShouldBe("Giornale");
        }

        [Fact]
        public async Task CreateMultiLingualMap_NoTranslations_ShouldStillMapObject()
        {
            /*
             *  When no translation is available, it should still map the
             */

            UsingDbContext(context =>
            {
                context.ProductTranslations.RemoveRange(context.ProductTranslations.ToList());
            });

            CultureInfo.CurrentUICulture = new CultureInfo("en-GB");

            var products = await _productAppService.GetProducts();
            products.ShouldNotBeNull();

            products.Items.Count.ShouldBe(3);
            var product1 = products.Items[0];
            var product2 = products.Items[1];
            var product3 = products.Items[2];

            product1.Language.ShouldBe(null);
            product1.Name.ShouldBe(null);
            product1.Price.ShouldBe(10);

            product2.Language.ShouldBe(null);
            product2.Name.ShouldBe(null);
            product2.Price.ShouldBe(99);

            product3.Language.ShouldBe(null);
            product3.Name.ShouldBe(null);
            product3.Price.ShouldBe(15);
        }

        [Fact]
        public async Task CreateMultiLingualMap_Tests_Dont_Override_MultiLingual_Entity_Id()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("tr");

            var products = await _productAppService.GetProducts();
            products.ShouldNotBeNull();

            var product1 = products.Items[0];

            product1.Language.ShouldBe("tr");
            product1.Name.ShouldBe("Saat");
            product1.Id.ShouldBe(1);
        }

        [Fact]
        public async Task Allow_Modifying_CreateMultiLingualMap_Mapping()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr");

            var orders = await _orderAppService.GetOrders();

            orders.Items.Count.ShouldBe(1);

            var testOrder = orders.Items.First();

            testOrder.Price.ShouldBe(100);
            testOrder.Language.ShouldBe("fr");
            testOrder.Name.ShouldBe("Tester");
            testOrder.ProductCount.ShouldBe(3);
        }
    }
}