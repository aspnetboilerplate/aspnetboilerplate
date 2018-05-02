using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.Shop;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.MultiLingual
{
    public class MultiLingual_Entity_Tests : SampleApplicationTestBase
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
                var product = await _productRepository.GetAll()
                                                .Include(p => p.Translations)
                                                .Where(p => p.Translations.Any(pt => pt.Language == "en" && pt.Name == "Mobile Phone"))
                                                .FirstOrDefaultAsync();

                product.ShouldNotBeNull();

                await _productAppService.UpdateProduct(new ProductUpdateDto
                {
                    Id = product.Id,
                    Price = product.Price,
                    Stock = product.Stock,
                    Translations = new List<ProductTranslationDto>
                    {
                        new ProductTranslationDto
                        {
                            Name = "Cell Phone",
                            Language = "en"
                        }
                    }
                });

                await uow.CompleteAsync();
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                // Old Translation
                var product = await _productRepository.GetAll()
                    .Include(p => p.Translations)
                    .Where(p => p.Translations.Any(pt => pt.Language == "en" && pt.Name == "Mobile Phone"))
                    .FirstOrDefaultAsync();

                product.ShouldBe(null);

                product = await _productRepository.GetAll()
                    .Include(p => p.Translations)
                    .Where(p => p.Translations.Any(pt => pt.Language == "en" && pt.Name == "Cell Phone"))
                    .FirstOrDefaultAsync();

                product.ShouldNotBe(null);
                product.Translations.Count.ShouldBe(1);

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Update_MultiLingualEntity_With_Multiple_Translation_Test()
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
                var product = await _productRepository.GetAll()
                                                .Include(p => p.Translations)
                                                .Where(p => p.Translations.Any(pt => pt.Language == "en" && pt.Name == "Mobile Phone"))
                                                .FirstOrDefaultAsync();

                product.ShouldNotBeNull();

                await _productAppService.UpdateProduct(new ProductUpdateDto
                {
                    Id = product.Id,
                    Price = product.Price,
                    Stock = product.Stock,
                    Translations = new List<ProductTranslationDto>
                    {
                        new ProductTranslationDto
                        {
                            Name = "Cell Phone",
                            Language = "en"
                        },
                        new ProductTranslationDto
                        {
                            Name = "Telefon",
                            Language = "tr"
                        }
                    }
                });

                await uow.CompleteAsync();
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                // Old Translation
                var product = await _productRepository.GetAll()
                    .Include(p => p.Translations)
                    .Where(p => p.Translations.Any(pt => pt.Language == "en" && pt.Name == "Mobile Phone"))
                    .FirstOrDefaultAsync();

                product.ShouldBe(null);

                product = await _productRepository.GetAll()
                    .Include(p => p.Translations)
                    .Where(p => p.Translations.Any(pt => pt.Language == "en" && pt.Name == "Cell Phone"))
                    .FirstOrDefaultAsync();

                product.ShouldNotBe(null);
                product.Translations.Count.ShouldBe(2);

                await uow.CompleteAsync();
            }
        }

        // todo@ismail -> Add Translate tests and refactor above tests...
    }
}