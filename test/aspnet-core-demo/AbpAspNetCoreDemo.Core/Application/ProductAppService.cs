using System.Collections.Generic;
using Abp.Application.Services;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;
using Abp.AutoMapper;

namespace AbpAspNetCoreDemo.Core.Application
{
    public class ProductAppService : ApplicationService, IProductAppService
    {
        private readonly IProductService _productService;

        public ProductAppService(IProductService productService)
        {
            _productService = productService;
        }

        public List<ProductDto> GetAll()
        {
            return _productService.GetAll().MapTo<List<ProductDto>>();
        }
    }
}
