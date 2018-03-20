using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;

namespace Abp.TestBase.SampleApplication.Shop
{
    public class ProductAppService : ApplicationService, IProductAppService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductAppService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ListResultDto<ProductListDto>> GetProducts()
        {
            var products = await _productRepository.GetAllIncluding(p => p.Translations).ToListAsync();
            return new ListResultDto<ProductListDto>(ObjectMapper.Map<List<ProductListDto>>(products));
        }
    }
}