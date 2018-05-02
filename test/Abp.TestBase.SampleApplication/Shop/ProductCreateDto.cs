using System.Collections.Generic;

namespace Abp.TestBase.SampleApplication.Shop
{
    public class ProductCreateDto
    {
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public ICollection<ProductTranslationDto> Translations { get; set; }
    }
}