using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace Abp.ZeroCore.SampleApp.Application.Shop
{
    public interface IProductAppService : IApplicationService
    {
        Task<ListResultDto<ProductListDto>> GetProducts();

        Task CreateProduct(ProductCreateDto input);

        Task UpdateProduct(ProductUpdateDto input);

        Task Translate(int productId, ProductTranslationDto input);
    }
}