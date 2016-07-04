using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;
using Abp.AutoMapper;
using Abp.Domain.Repositories;

namespace AbpAspNetCoreDemo.Core.Application
{
    public class ProductAppService : ApplicationService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductAppService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public virtual List<ProductDto> GetAll()
        {
            return _productRepository.GetAll().ToList().MapTo<List<ProductDto>>();
        }

        public virtual int CreateProduct(ProductCreateInput input)
        {
            return _productRepository.InsertAndGetId(ObjectMapper.Map<Product>(input));
        }
    }
}
