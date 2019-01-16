using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AspNetCore.App.AppServices;
using Abp.AspNetCore.App.Controllers;
using Abp.Json;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class WrapResultNamingStrategy_Tests : AppTestBase
    {
        [Fact]
        public async Task WrapResultNamingStrategy_CamelCaseNamingStrategy_Test()
        {
            // Act
            var response = await GetResponseAsStringAsync("api/services/app" + GetUrl<WrapResultAppService>(nameof(WrapResultAppService.Get)));
            var serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();
            serializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            // Assert
            response.ShouldBe((new AjaxResponse<WrapResultDto>(WrapResultDto.Create())
            {
                Error = null,
                Success = true,
                TargetUrl = null,
                UnAuthorizedRequest = false,
            }) .ToJsonString(serializerSettings));
        }

        [Fact]
        public async Task WrapResultNamingStrategy_DefaultNamingStrategy_Test()
        {
            // Act
            var response = await GetResponseAsStringAsync("api/services/app" + GetUrl<WrapResultAppService>(nameof(WrapResultAppService.Get2)));
            var serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();
            serializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new DefaultNamingStrategy()
            };

            // Assert
            response.ShouldBe((new AjaxResponse<WrapResultDto>(WrapResultDto.Create())
            {
                Error = null,
                Success = true,
                TargetUrl = null,
                UnAuthorizedRequest = false,
            }).ToJsonString(serializerSettings));
        }
    }
}
