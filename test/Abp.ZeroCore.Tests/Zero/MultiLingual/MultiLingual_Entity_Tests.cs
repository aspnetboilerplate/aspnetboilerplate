using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Application.Shop;
using Abp.ZeroCore.SampleApp.Core.Shop;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.Zero.MultiLingual
{
    public class MultiLingual_Entity_Tests : AbpZeroTestBase
    {
        private readonly IProductAppService _productAppService;
        private readonly IRepository<Product> _productRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MultiLingual_Entity_Tests()
        {
            _productAppService = Resolve<IProductAppService>();
            _productRepository = Resolve<IRepository<Product>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Create_MultiLingualEntity_With_Single_Translation_Test()
        {
            await _productAppService.CreateProduct(new ProductCreateDto
            {
                Price = 99,
                Stock = 1000,
                Translations = new List<ProductTranslationDto>
                {
                    new ProductTranslationDto
                    {
                        Language = "en",
                        Name = "Mobile Phone"
                    }
                }
            });

            using (var uow = _unitOfWorkManager.Begin())
            {
                var products = await _productRepository.GetAllIncluding(p => p.Translations).ToListAsync();
                products.SelectMany(p => p.Translations).Count(pt => pt.Name == "Mobile Phone" && pt.Language == "en").ShouldBe(1);

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Create_MultiLingualEntity_With_Multiple_Translation_Test()
        {
            await _productAppService.CreateProduct(new ProductCreateDto
            {
                Price = 99,
                Stock = 1000,
                Translations = new List<ProductTranslationDto>
                {
                    new ProductTranslationDto
                    {
                        Language = "en",
                        Name = "Mobile Phone"
                    },
                    new ProductTranslationDto
                    {
                        Language = "tr",
                        Name = "Cep telefonu"
                    }
                }
            });

            using (var uow = _unitOfWorkManager.Begin())
            {
                var products = await _productRepository.GetAllIncluding(p => p.Translations).ToListAsync();

                products.SelectMany(p => p.Translations).Count(pt => pt.Name == "Mobile Phone" && pt.Language == "en").ShouldBe(1);
                products.SelectMany(p => p.Translations).Count(pt => pt.Name == "Cep telefonu" && pt.Language == "tr").ShouldBe(1);

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Update_MultiLingualEntity_With_Single_Translation_Test()
        {
            var product = await GetProduct("it", "Giornale");

            product.ShouldNotBeNull();
            product.Translations.Count.ShouldBe(1);

            await _productAppService.UpdateProduct(new ProductUpdateDto
            {
                Id = product.Id,
                Price = product.Price,
                Stock = product.Stock,
                Translations = new List<ProductTranslationDto>
                {
                    new ProductTranslationDto
                    {
                        Name = "Newspaper",
                        Language = "en"
                    }
                }
            });

            using (var uow = _unitOfWorkManager.Begin())
            {
                // Old Translation
                product = await GetProduct("it", "Giornale");

                product.ShouldBe(null);

                product = await GetProduct("en", "Newspaper");

                product.ShouldNotBe(null);
                product.Translations.Count.ShouldBe(1);

                var translation = product.Translations.First();
                translation.Language.ShouldBe("en");
                translation.Name.ShouldBe("Newspaper");

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Update_MultiLingualEntity_With_Multiple_Translation_Test()
        {
            var product = await GetProduct("en", "Bike");

            product.ShouldNotBeNull();
            product.Translations.Count.ShouldBe(2);

            await _productAppService.UpdateProduct(new ProductUpdateDto
            {
                Id = product.Id,
                Price = product.Price,
                Stock = product.Stock,
                Translations = new List<ProductTranslationDto>
                {
                    new ProductTranslationDto
                    {
                        Name = "Bicycle",
                        Language = "en"
                    },
                    new ProductTranslationDto
                    {
                        Name = "Bicicleta",
                        Language = "es"
                    }
                }
            });

            using (var uow = _unitOfWorkManager.Begin())
            {
                // Old Translation
                product = await GetProduct("en", "Bike");

                product.ShouldBe(null);

                product = await GetProduct("en", "Bicycle");

                product.ShouldNotBe(null);
                product.Translations.Count.ShouldBe(2);

                product.Translations.Count(pt => pt.Language == "fr").ShouldBe(0);
                product.Translations.Count(pt => pt.Language == "es" && pt.Name == "Bicicleta").ShouldBe(1);

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Translate_MultiLingualEntity_Insert_Test()
        {
            var product = await GetProduct("it", "Giornale");

            await _productAppService.Translate(product.Id, new ProductTranslationDto
            {
                Name = "Bicycle",
                Language = "en"
            });

            product = await GetProduct("en", "Bicycle");
            product.ShouldNotBeNull();
            product.Translations.Count.ShouldBe(2);
            product.Translations.Count(pt => pt.Language == "en" && pt.Name == "Bicycle").ShouldNotBeNull();
        }

        [Fact]
        public async Task Translate_MultiLingualEntity_Update_Test()
        {
            var product = await GetProduct("it", "Giornale");

            await _productAppService.Translate(product.Id, new ProductTranslationDto
            {
                Name = "il Giornale",
                Language = "it"
            });

            product = await GetProduct("it", "il Giornale");
            product.ShouldNotBeNull();
            product.Translations.Count.ShouldBe(1);
        }

        private async Task<Product> GetProduct(string culture, string productName)
        {
            Product product;

            using (var uow = _unitOfWorkManager.Begin())
            {
                product = await _productRepository.GetAll()
                    .Include(p => p.Translations)
                    .Where(p => p.Translations.Any(pt => pt.Language == culture && pt.Name == productName))
                    .FirstOrDefaultAsync();

                await uow.CompleteAsync();
            }

            return product;
        }
    }
}