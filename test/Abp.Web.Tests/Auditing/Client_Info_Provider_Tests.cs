using System.Collections.Specialized;
using System.Web;
using Abp.Auditing;
using Abp.TestBase;
using NSubstitute;
using NSubstitute.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Web.Tests.Auditing
{
    public class Client_Info_Provider_Tests : AbpIntegratedTestBase<AbpWebModule>
    {
        private readonly WebClientInfoProvider _clientInfoProvider;

        public Client_Info_Provider_Tests()
        {
            _clientInfoProvider = Substitute.ForPartsOf<WebClientInfoProvider>();
        }
        
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("::1", "::1")]
        [InlineData("127.0.0.1", "127.0.0.1")]
        [InlineData("0:0:0:0:0:0:0:1", "::1")]
        public void Should_Save_Remote_Address_From_Server_Variable(string remote_address, string clientIpAddress)
        {
            var serverVariables = new NameValueCollection
            {
                { "REMOTE_ADDR", remote_address }
            };

            MockHttpContext(serverVariables);

            _clientInfoProvider.ClientIpAddress.ShouldBe(clientIpAddress);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("::1", "::1")]
        [InlineData("127.0.0.1", "127.0.0.1")]
        [InlineData("127.0.0.1, 192.168.1.1", "127.0.0.1")]
        [InlineData("127.0.0.1:8888", "127.0.0.1")]
        [InlineData("0:0:0:0:0:0:0:1", "::1")]
        [InlineData("0:0:0:0:0:0:0:1, 0:0:0:0:0:ffff:c0a8:101", "::1")]
        [InlineData("[0:0:0:0:0:0:0:1]:8888", "::1")]
        public void Should_Save_Http_X_Forwarded_For_From_Server_Variable(string forwarded_for, string clientIpAddress)
        {
            var serverVariables = new NameValueCollection
            {
                { "HTTP_X_FORWARDED_FOR", forwarded_for }
            };

            MockHttpContext(serverVariables);

            _clientInfoProvider.ClientIpAddress.ShouldBe(clientIpAddress);
        }

        private void MockHttpContext(NameValueCollection serverVariables = null)
        {
            var mockHttpRequest = Substitute.For<HttpRequestBase>();
            if (serverVariables != null)
            {
                mockHttpRequest.ServerVariables.Returns(serverVariables);
            }
            var mockHttpContext = Substitute.For<HttpContextBase>();
            mockHttpContext.Request.Returns(mockHttpRequest);

            _clientInfoProvider.Configure().GetCurrentHttpContext().Returns(mockHttpContext);
        }
    }
}
