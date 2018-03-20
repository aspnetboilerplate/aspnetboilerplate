using Abp.AutoMapper;

namespace Abp.TestBase.SampleApplication.Shop
{
    [AutoMapFrom(typeof(Product), typeof(ProductTranslation))]
    public class ProductListDto
    {
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }
    }
}