using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.ControllersOther
{
    [Controller]
    [Route("api/mydemoservice/[action]")]
    public class MyDemoAppService : ApplicationService
    {
        private readonly IRepository<Product> _productRepository;

        public MyDemoAppService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        [Route("test1Method")]
        [Route("test1MethodRoute2")]
        [HttpPut]
        public List<ProductDto> MyTest1(MyDemoInputDto input)
        {
            //throw new UserFriendlyException("just testing...");
            return _productRepository.GetAll().ToList().MapTo<List<ProductDto>>();
        }

        [HttpGet]
        public int GetMe(MyDemoInputDto input)
        {
            return 42;
        }

        public int Test2()
        {
            return 42;
        }

        public int Test3([FromHeader(Name = "my-header-value")] string myHeaderValue)
        {
            return 42;
        }

        [HttpPost]
        public int Test4(IFormFile formFile)
        {
            return 42;
        }

        public int GetTest5([FromQuery(Name = "prm1")]int p1, string p2)
        {
            return 42;
        }
    }

    public class MyDemoInputDto : IInputDto
    {
        [Required]
        public string TestStr { get; set; }

        [Range(1, 1000)]
        public int TestInt { get; set; }

        public MyInnerInput InnrInput { get; set; }
    }

    public class MyInnerInput
    {
        public bool InnerTestBool { get; set; }

        public byte InnerTestByte { get; set; }
    }
}
