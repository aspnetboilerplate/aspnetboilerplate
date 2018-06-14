using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Abp.ZeroCore.SampleApp.Application.Shop
{
    public class ProductUpdateDto: EntityDto
    {
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public ICollection<ProductTranslationDto> Translations { get; set; }
    }
}