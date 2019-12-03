using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.Json;
using Abp.Web.Models;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class Validation_Tests : AppTestBase
    {
        [Fact]
        public async Task Should_Work_With_Valid_Parameters_ActionResult()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetContentValue),
                    new { value = 42 }
                )
            );

            response.ShouldBe("OK: 42");
        }

        [Fact]
        public async Task Should_Work_With_Valid_Parameters_JsonResult()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<ValidationTestController.ValidationTestArgument1>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValue),
                    new { value = 42 }
                )
            );

            response.Success.ShouldBeTrue();
            response.Result.Value.ShouldBe(42);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData("undefined")]
        [InlineData(null)]
        public async Task Should_Not_Work_With_Invalid_Parameters(object value)
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<ValidationTestController.ValidationTestArgument1>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValue),
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
            var response = await GetResponseAsObjectAsync<AjaxResponse<ValidationTestController.ValidationTestArgument1>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValue)
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
        public async Task Should_Not_Work_With_Invalid_Parameters_Enum()
        {
            // Act
            var response = await PostAsync<AjaxResponse<ValidationTestController.ValidationTestArgument2>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValueWithEnum)
                ),
                new StringContent("{ \"value\": \"asd\" }", Encoding.UTF8, "application/json"),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.Length.ShouldBe(1);
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("$.value");
        }

        [Fact]
        public async Task Should_Not_Work_With_Invalid_Parameters_Validatable_Object()
        {
            // Act
            var response = await PostAsync<AjaxResponse<ValidationTestController.ValidationTestArgument3>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValueWithValidatableObject)
                ),
                new StringContent("{ \"value\": -1 }", Encoding.UTF8, "application/json"),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.Length.ShouldBe(1);
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("value");
            response.Error.ValidationErrors[0].Message.ShouldBe("Value must be higher than 0");
        }

        [Fact]
        public async Task Should_Not_Work_With_Invalid_Parameters_Custom_Validate()
        {
            // Act
            var response = await PostAsync<AjaxResponse<ValidationTestController.ValidationTestArgument4>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValueWithCustomValidate)
                ),
                new StringContent("{ \"value\": \"asd\" }", Encoding.UTF8, "application/json"),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.Length.ShouldBe(1);
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("value");
            response.Error.ValidationErrors[0].Message.ShouldBe("Value must be \"abp\"");
        }

        [Fact]
        public async Task Should_Not_Work_With_Invalid_Parameters_Combined_Validators()
        {
            // Act
            var response = await PostAsync<AjaxResponse<ValidationTestController.ValidationTestArgument5>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValueWithCombinedValidators)
                ),
                new StringContent("{ \"value\": -1 }", Encoding.UTF8, "application/json"),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.Length.ShouldBe(2);
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("value");
            response.Error.ValidationErrors[1].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[1].Members[0].ShouldBe("value");
        }
    }
}
