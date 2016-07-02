using System.Collections.Generic;
using Abp.Application.Services;
using Abp.UI;
using AbpAspNetCoreDemo.Core.Application.Dtos;

namespace AbpAspNetCoreDemo.Core.Application
{
    //Alternative to ProductsController
    public class Products2AppService : ApplicationService
    {
        private readonly IProductAppService _productAppService;

        public Products2AppService(
            IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        public IEnumerable<ProductDto> GetAll()
        {
            return _productAppService.Get();
        }

        public string Get(int id, string y)
        {
            throw new UserFriendlyException("A test exception message");
            return "value";
        }

        public void Post(string value)
        {
        }

        public void Put(int id, string value)
        {
        }

        public void Delete(int id)
        {
        }
    }
}