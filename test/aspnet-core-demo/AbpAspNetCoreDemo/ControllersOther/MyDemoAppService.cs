using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.ControllersOther
{
    [Controller]
    public class MyDemoAppService : ApplicationService
    {
        private readonly IRepository<Product> _productRepository;

        public MyDemoAppService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost]
        public List<ProductDto> MyTest1(MyDemoInputDto input)
        {
            //throw new UserFriendlyException("just testing...");
            return _productRepository.GetAll().ToList().MapTo<List<ProductDto>>();
        }
    }

    public class MyDemoInputDto : IInputDto
    {
        [Required]
        public string TestStr { get; set; }

        [Range(1,1000)]
        public int TestInt { get; set; }
    }
}
