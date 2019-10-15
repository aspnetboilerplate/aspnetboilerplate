using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.ZeroCore.SampleApp.Core.Shop;
using Microsoft.EntityFrameworkCore;

namespace Abp.ZeroCore.SampleApp.Application.Shop
{
    public class ProductAppService : ApplicationService, IProductAppService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductTranslation> _productTranslationRepository;

        public ProductAppService(
            IRepository<Product> productRepository,
            IRepository<ProductTranslation> productTranslationRepository
        )
        {
            _productRepository = productRepository;
            _productTranslationRepository = productTranslationRepository;
        }

        public async Task<ListResultDto<ProductListDto>> GetProducts()
        {
            var products = await _productRepository.GetAllIncluding(p => p.Translations).ToListAsync();
            return new ListResultDto<ProductListDto>(ObjectMapper.Map<List<ProductListDto>>(products));
        }

        public async Task CreateProduct(ProductCreateDto input)
        {
            var product = ObjectMapper.Map<Product>(input);
            await _productRepository.InsertAsync(product);
        }

        public async Task UpdateProduct(ProductUpdateDto input)
        {
            var product = await _productRepository.GetAllIncluding(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == input.Id);

            product.Translations.Clear();

            ObjectMapper.Map(input, product);
        }

        public async Task Translate(int productId, ProductTranslationDto input)
        {
            var translation = await _productTranslationRepository.GetAll()
                                                           .FirstOrDefaultAsync(pt => pt.CoreId == productId && pt.Language == input.Language);

            if (translation == null)
            {
                var newTranslation = ObjectMapper.Map<ProductTranslation>(input);
                newTranslation.CoreId = productId;

                await _productTranslationRepository.InsertAsync(newTranslation);
            }
            else
            {
                ObjectMapper.Map(input, translation);
            }
        }
    }
}