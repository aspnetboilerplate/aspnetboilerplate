using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace Abp.ZeroCore.SampleApp.Application.Shop
{
    public interface IOrderAppService : IApplicationService
    {
        Task<ListResultDto<OrderListDto>> GetOrders();
    }
}