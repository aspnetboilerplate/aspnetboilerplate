using Abp.Application.Services.Dto;

namespace Yishe.Abp.Application.Services.Dto
{
    public interface IPagedRequest: IPagedResultRequest
    {
         int PageSize { get; set; }
       
    }
}