using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;

namespace AbpAspNetCoreDemo.Core.Application
{
    public class ProductAppService : ApplicationService
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

        public int CreateProduct(ProductCreateInput input)
        {
            return _productRepository.InsertAndGetId(ObjectMapper.Map<Product>(input));
        }

        public void CreateProductAndRollback(ProductCreateInput input)
        {
            _productRepository.Insert(ObjectMapper.Map<Product>(input));
            CurrentUnitOfWork.SaveChanges();
            throw new UserFriendlyException("This exception is thrown to rollback the transaction!");
        }
    }
}
