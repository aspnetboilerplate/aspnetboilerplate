using System.Collections.Generic;
using Abp.Application.Services;
using AbpAspNetCoreDemo.Core.Application.Dtos;

namespace AbpAspNetCoreDemo.Core.Application
{
    public interface IProductAppService : IApplicationService
    {
        List<ProductDto> Get();
    }
}