using System.Collections.Generic;
using Abp.Application.Services;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.UI;
using AbpAspNetCoreDemo.Core.Application;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AbpAspNetCoreDemo.Controllers
{
    [Route("api/[controller]")]
    [RemoteService]
    public class ProductsController : AbpController
    {
        private readonly IProductAppService _productAppService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductAppService productAppService,
            ILogger<ProductsController> logger)
        {
            _productAppService = productAppService;
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<ProductDto> GetAll()
        {
            _logger.LogInformation("ProductsController.Get method is called. This message is logged via Microsoft.Extensions.Logging.ILogger");
            return _productAppService.Get();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id, string y)
        {
            throw new UserFriendlyException("A test exception message");
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
