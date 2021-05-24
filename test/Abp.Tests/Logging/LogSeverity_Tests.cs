using Abp.Authorization;
using Abp.Logging;
using Abp.Runtime.Validation;
using Abp.UI;
using Shouldly;
using Xunit;

namespace Abp.Tests.Logging
{
    public class LogSeverity_Tests: TestBaseWithLocalIocManager
    {
        [Fact]
        public void AuthorizationException_Default_Log_Severity_Test()
        {
            // change log severity ...
            AbpAuthorizationException.DefaultLogSeverity = LogSeverity.Warn;
            
            var exception = new AbpAuthorizationException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Warn);
        }

        [Fact]
        public void AuthorizationException_Default_Log_Severity_Change_Test()
        {
            // change log severity ...
            AbpAuthorizationException.DefaultLogSeverity = LogSeverity.Error;
            
            var exception = new AbpAuthorizationException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Error);
        }
        
        [Fact]
        public void ValidationException_Default_Log_Severity_Test()
        {
            // change log severity ...
            AbpValidationException.DefaultLogSeverity = LogSeverity.Warn;
            
            var exception = new AbpValidationException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Warn);
        }

        [Fact]
        public void ValidationException_Default_Log_Severity_Change_Test()
        {
            // change log severity ...
            AbpValidationException.DefaultLogSeverity = LogSeverity.Error;
            
            var exception = new AbpValidationException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Error);
        }
        
        [Fact]
        public void UserFriendlyException_Default_Log_Severity_Test()
        {
            // change log severity ...
            UserFriendlyException.DefaultLogSeverity = LogSeverity.Warn;
            
            var exception = new UserFriendlyException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Warn);
        }

        [Fact]
        public void UserFriendlyException_Default_Log_Severity_Change_Test()
        {
            // change log severity ...
            UserFriendlyException.DefaultLogSeverity = LogSeverity.Error;
            
            var exception = new UserFriendlyException("Test exception !");
            exception.Severity.ShouldBe(LogSeverity.Error);
        }
    }
}
