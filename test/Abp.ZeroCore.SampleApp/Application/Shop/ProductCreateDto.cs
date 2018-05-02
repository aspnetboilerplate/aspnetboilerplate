using System.Collections.Generic;

namespace Abp.ZeroCore.SampleApp.Application.Shop
{
    public class ProductCreateDto
    {
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public ICollection<ProductTranslationDto> Translations { get; set; }
    }
}