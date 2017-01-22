using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.Web.Models;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class NameConflict_Tests : AppTestBase
    {
        [Fact]
        public async Task Url_Action_Should_Return_Controller_Path_By_Default()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                GetUrl<NameConflictController>(
                    nameof(NameConflictController.GetSelfActionUrl)
                )
            );

            //Assert
            response.Result.ShouldBe("/NameConflict/GetSelfActionUrl");
        }

        [Fact]
        public async Task Url_Action_Should_Return_Controller_Path_With_Area_Name()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                GetUrl<NameConflictController>(
                    nameof(NameConflictController.GetAppServiceActionUrlWithArea)
                )
            );

            //Assert
            response.Result.ShouldBe("/api/services/app/NameConflict/GetConstantString");
        }

        [Fact]
        public async Task Should_Use_App_Service_With_Full_Route()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                "/api/services/app/NameConflict/GetConstantString"
            );

            //Assert
            response.Result.ShouldBe("return-value-from-app-service");
        }
    }
}