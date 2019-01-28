using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

namespace Abp.AspNetCore.App.AppServices
{
    public class WrapResultDto : EntityDto
    {
        public static WrapResultDto Create()
        {
            return new WrapResultDto
            {
                Id = 1,
                Name = "john",
                Age = 8,
                Sex = true
            };
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public bool Sex { get; set; }
    }


    public class WrapResultAppService : ApplicationService
    {
        [HttpGet]
        [WrapResult(namingStrategyType: typeof(CamelCaseNamingStrategy))]
        public WrapResultDto Get()
        {
            return WrapResultDto.Create();
        }

        [HttpGet]
        [WrapResult(namingStrategyType: typeof(DefaultNamingStrategy))]
        public WrapResultDto Get2()
        {
            return WrapResultDto.Create();
        }
    }
}
