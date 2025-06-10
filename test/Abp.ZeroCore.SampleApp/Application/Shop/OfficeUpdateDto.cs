using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Abp.ZeroCore.SampleApp.Application.Shop;

public class OfficeUpdateDto : EntityDto
{
    public int Capacity { get; set; }

    public ICollection<ProductTranslationDto> Translations { get; set; }
}