using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.Web.Models;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class FluentValidation_Tests : AppTestBase
    {
        [Fact]
        public async Task Should_Work_With_Valid_Parameters()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<FluentValidationTestController.ValidationTestArgument1>>(
                GetUrl<FluentValidationTestController>(
                    nameof(FluentValidationTestController.GetJsonValue),
                    new { value = 42 }
                )
            );

            response.Success.ShouldBeTrue();
            response.Result.Value.ShouldBe(42);
        }

        [Fact]
        public async Task Should_Not_Throw_Exception_For_Nullable_Values_With_Null_Parameter()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<FluentValidationTestController.ValidationTestArgument3>>(
                GetUrl<FluentValidationTestController>(
                    nameof(FluentValidationTestController.GetNullableJsonValue),
                    new FluentValidationTestController.ValidationTestArgument3 { Value = null }
                )
            );

            response.Success.ShouldBeTrue();
            response.Result.Value.ShouldBe(null);
        }

        [Fact]
        public async Task Should_Not_Throw_Exception_For_Nullable_Values_With_Null_Parameter_Post()
        {
            // Act
            var response = await PostAsync<AjaxResponse<FluentValidationTestController.ValidationTestArgument3>>(
                GetUrl<FluentValidationTestController>(
                    nameof(FluentValidationTestController.GetNullableJsonValue2)
                ),
                new StringContent("{ \"value\": null }", Encoding.UTF8, "application/json")
            );

            response.Success.ShouldBeTrue();
            response.Result.Value.ShouldBe(null);
        }

        [Fact]
        public async Task Should_Work_With_Valid_Array_Parameter()
        {
            // Act
            var response = await PostAsync<AjaxResponse<FluentValidationTestController.ValidationTestArgument2>>(
                GetUrl<FluentValidationTestController>(
                    nameof(FluentValidationTestController.GetJsonValueWithArray)
                ),
                new StringContent("{ \"array\": [ { \"value\": 1}, { \"value\": 3}, { \"value\": 5}] }", Encoding.UTF8, "application/json")
            );

            response.Success.ShouldBeTrue();
            response.Result.Array.Length.ShouldBe(3);
            response.Result.Array[0].Value.ShouldBe(1);
            response.Result.Array[1].Value.ShouldBe(3);
            response.Result.Array[2].Value.ShouldBe(5);
        }

        [Fact]
        public async Task Should_Not_Work_With_Invalid_Array_Parameter()
        {
            // Act
            var response = await PostAsync<AjaxResponse<FluentValidationTestController.ValidationTestArgument2>>(
                GetUrl<FluentValidationTestController>(
                    nameof(FluentValidationTestController.GetJsonValueWithArray)
                ),
                new StringContent("{ \"array\": [ { \"value\": 1}, { \"value\": -3}] }", Encoding.UTF8, "application/json"),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors.Length.ShouldBe(2);
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("array");
            response.Error.ValidationErrors[0].Message.ShouldBe("Array must contain at least three items");
            response.Error.ValidationErrors[1].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[1].Members[0].ShouldBe("value");
        }

        [Theory]
        [InlineData(-2)]
        [InlineData("undefined")]
        [InlineData(null)]
        public async Task Should_Not_Work_With_Invalid_Parameters(object value)
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<FluentValidationTestController.ValidationTestArgument1>>(
                GetUrl<FluentValidationTestController>(
                    nameof(FluentValidationTestController.GetJsonValue),
                    new { value = value }
                ),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("value");
        }

        [Fact]
        public async Task Should_Not_Work_With_Invalid_Parameters_No_Parameter_Provided()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<FluentValidationTestController.ValidationTestArgument1>>(
                GetUrl<FluentValidationTestController>(
                    nameof(FluentValidationTestController.GetJsonValue)
                ),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("value");
        }
    }
}
