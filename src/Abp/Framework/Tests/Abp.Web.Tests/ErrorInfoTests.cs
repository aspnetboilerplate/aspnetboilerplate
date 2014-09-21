using System;
using Abp.UI;
using Abp.Web.Models;
using Xunit;

namespace Abp.Web.Tests
{
    public class ErrorInfoTests
    {
        static ErrorInfoTests()
        {
            ErrorInfo.AddExceptionConverter(new MyErrorInfoConverter());
        }

        [Fact]
        public void Should_Convert_Specific_Exception()
        {
            var errorInfo = ErrorInfo.ForException(new MySpecificException());
            Assert.Equal(42, errorInfo.Code);
            Assert.Equal("MySpecificMessage", errorInfo.Message);
            Assert.Equal("MySpecificMessageDetails", errorInfo.Details);
        }

        [Fact]
        public void Should_Convert_UserFriendlyException()
        {
            var errorInfo = ErrorInfo.ForException(new UserFriendlyException("Test message"));
            Assert.Equal(0, errorInfo.Code);
            Assert.Equal("Test message", errorInfo.Message);
        }

        public class MySpecificException : Exception
        {
            
        }

        public class MyErrorInfoConverter : IExceptionToErrorInfoConverter
        {
            public IExceptionToErrorInfoConverter Next { set; private get; }

            public ErrorInfo Convert(Exception exception)
            {
                if (exception is MySpecificException)
                {
                    return new ErrorInfo(42, "MySpecificMessage", "MySpecificMessageDetails");
                }

                return Next.Convert(exception);
            }
        }
    }
}
