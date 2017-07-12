using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.TestBase;
using Abp.Web.Configuration;
using Abp.Web.Models;
using Shouldly;
using Xunit;

namespace Abp.Web.Common.Tests.Web
{
    public class DefaultErrorInfoConverter_Tests : AbpIntegratedTestBase<AbpWebCommonTestModule>
    {
        private readonly DefaultErrorInfoConverter _defaultErrorInfoConverter;

        public DefaultErrorInfoConverter_Tests()
        {
            _defaultErrorInfoConverter = Resolve<DefaultErrorInfoConverter>();
        }

        [Fact]
        public async Task DefaultErrorInfoConverter_Should_Work_For_EntityNotFoundException_Overload_Methods()
        {
            var message = "Test message";
            var errorInfo = _defaultErrorInfoConverter.Convert(new EntityNotFoundException(message));

            Assert.Equal(errorInfo.Message, message);

            var exception = new EntityNotFoundException();
            errorInfo = _defaultErrorInfoConverter.Convert(exception);

            Assert.Equal(errorInfo.Message, exception.Message);
        }
    }
}
