using System.Collections.Generic;

namespace Abp.ZeroCore.SampleApp.Application.Shop;

public class OfficeCreateDto
{
    public int Capacity { get; set; }

    public ICollection<ProductTranslationDto> Translations { get; set; }
}