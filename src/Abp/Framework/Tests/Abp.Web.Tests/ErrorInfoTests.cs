using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using Abp.Web.Models;
using NUnit.Framework;

namespace Abp.Web.Tests
{
    [TestFixture]
    public class ErrorInfoTests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            ErrorInfo.AddExceptionConverter(new MyErrorInfoConverter());
        }

        [Test]
        public void Should_Convert_Specific_Exception()
        {
            var errorInfo = ErrorInfo.ForException(new MySpecificException());
            Assert.AreEqual(42, errorInfo.Code);
            Assert.AreEqual("MySpecificMessage", errorInfo.Message);
            Assert.AreEqual("MySpecificMessageDetails", errorInfo.Details);
        }

        [Test]
        public void Should_Convert_UserFriendlyException()
        {
            var errorInfo = ErrorInfo.ForException(new UserFriendlyException("Test message"));
            Assert.AreEqual(0, errorInfo.Code);
            Assert.AreEqual("Test message", errorInfo.Message);
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
