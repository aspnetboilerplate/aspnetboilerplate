using System;
using Abp.Configuration.Startup;
using Abp.Dependency.Installers;
using Abp.Localization;
using Abp.Tests;
using Abp.UI;
using Abp.Web.Configuration;
using Abp.Web.Models;
using NSubstitute;
using Xunit;

namespace Abp.Web.Tests
{
    public class ErrorInfoBuilder_Tests : TestBaseWithLocalIocManager
    {
        private readonly IErrorInfoBuilder _errorInfoBuilder;

        public ErrorInfoBuilder_Tests()
        {
            LocalIocManager.IocContainer.Install(new AbpCoreInstaller());

            var configuration = LocalIocManager.Resolve<AbpStartupConfiguration>();
            configuration.Initialize();
            configuration.Localization.IsEnabled = false;

            _errorInfoBuilder = new ErrorInfoBuilder(Substitute.For<IAbpWebCommonModuleConfiguration>(), NullLocalizationManager.Instance);
            _errorInfoBuilder.AddExceptionConverter(new MyErrorInfoConverter());
        }

        [Fact]
        public void Should_Convert_Specific_Exception()
        {
            var errorInfo = _errorInfoBuilder.BuildForException(new MySpecificException());
            Assert.Equal(42, errorInfo.Code);
            Assert.Equal("MySpecificMessage", errorInfo.Message);
            Assert.Equal("MySpecificMessageDetails", errorInfo.Details);
        }

        [Fact]
        public void Should_Convert_UserFriendlyException()
        {
            var errorInfo = _errorInfoBuilder.BuildForException(new UserFriendlyException("Test message"));
            Assert.Equal(0, errorInfo.Code);
            Assert.Equal("Test message", errorInfo.Message);
        }

        //[Fact]
        //public void Should_Not_Convert_Other_Exceptions()
        //{
        //    var errorInfo = _errorInfoBuilder.BuildForException(new Exception("Test message"));
        //    Assert.Equal(0, errorInfo.Code);
        //    Assert.NotEqual("Test message", errorInfo.Message);
        //}

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
