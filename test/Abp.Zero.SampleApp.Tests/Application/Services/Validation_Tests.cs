using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Localization;
using Abp.Runtime.Validation;
using Abp.Zero.SampleApp.Users;
using Abp.Zero.SampleApp.Users.Dto;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Application.Services
{
    public class Validation_Tests : SampleAppTestBase
    {
        private readonly IUserAppService _userAppService;

        public Validation_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
        }


        [Theory]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("en-GB")]
        public void CustomValidationContext_Localize_Test(string cultureName)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);

            var exception = Assert.Throws<AbpValidationException>(() =>
            {
                _userAppService.CustomValidateMethod(new CustomValidateMethodInput());
            });

            exception.ValidationErrors.ShouldNotBeNull();
            exception.ValidationErrors.Count.ShouldBe(1);
            exception.ValidationErrors[0].ErrorMessage.ShouldBe("User is not in role.");
        }
    }
}
