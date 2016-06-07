using System.Collections.Generic;
using Abp.Domain.Services;

namespace AbpAspNetCoreDemo.Core.Domain
{
    public interface IProductService : IDomainService
    {
        List<Product> GetAll();
    }
}