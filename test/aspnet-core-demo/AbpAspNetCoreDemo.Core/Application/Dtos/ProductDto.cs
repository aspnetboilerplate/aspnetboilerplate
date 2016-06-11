using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AbpAspNetCoreDemo.Core.Domain;

namespace AbpAspNetCoreDemo.Core.Application.Dtos
{
    [AutoMap(typeof(Product))]
    public class ProductDto : EntityDto
    {
        public string Name { get; set; }

        public float Price { get; set; }
    }
}
