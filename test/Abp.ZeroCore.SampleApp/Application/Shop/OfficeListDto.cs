using Abp.Application.Services.Dto;

namespace Abp.ZeroCore.SampleApp.Application.Shop;

public class OfficeListDto : EntityDto
{
    public decimal Capacity { get; set; }

    public string Name { get; set; }

    public string Language { get; set; }
}