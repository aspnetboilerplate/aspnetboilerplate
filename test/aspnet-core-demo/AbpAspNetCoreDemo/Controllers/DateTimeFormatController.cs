using System;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers;

public class DateTimeFormatController : DemoControllerBase
{
    [Route("api/date-time-format/test")]
    [HttpPost]
    public string TestGetArray(TestInputDto model)
    {
        return model.TestDate.ToString("yyyy-MM-dd");
    }

    public class TestInputDto : EntityDto
    {
        public DateTime TestDate { get; set; }
    }
}