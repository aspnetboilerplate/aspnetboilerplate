using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;
using Abp.AutoMapper;
using Abp.Domain.Repositories;

namespace AbpAspNetCoreDemo.Core.Application
{
    public class ProductAppService : ApplicationService, IProductAppService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductAppService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public List<ProductDto> GetAll()
        {
            return _productRepository.GetAll().ToList().MapTo<List<ProductDto>>();
        }
    }
}
