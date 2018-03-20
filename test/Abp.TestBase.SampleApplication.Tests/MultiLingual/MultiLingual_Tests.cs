using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.TestBase.SampleApplication.Shop;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.MultiLingual
{
    public class MultiLingual_Tests: SampleApplicationTestBase
    {
        private readonly IProductAppService _productAppService;

        public MultiLingual_Tests()
        {
            _productAppService = Resolve<IProductAppService>();
        }

        [Fact]
        public async Task Get_MultiLingual_Dto_Test()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("tr");

            var products = await _productAppService.GetProducts();
            products.ShouldNotBeNull();

            products.Items.Count.ShouldBe(2);
            var product1 = products.Items[0];
            var product2 = products.Items[1];

            product1.Language.ShouldBe("tr");
            product1.Name.ShouldBe("Saat");

            product2.Language.ShouldBe("en");
            product2.Name.ShouldBe("Bike");
        }
    }
}
