using Abp.Web.Http;
using Shouldly;
using Xunit;

namespace Abp.Tests.Web.Http
{
    public class UrlHelper_Tests : TestBaseWithLocalIocManager
    {
        public UrlHelper_Tests()
        {
            LocalIocManager.Register<IUrlHelper, AbpUrlHelper>();
        }

        [Theory]
        [InlineData("/relative/path", "/relative/path")]
        [InlineData("//relative/path", "//relative/path")]
        [InlineData("/relative/path?id=1&value=2", "/relative/path?id=1&value=2")]
        [InlineData("%2F%E7%B5%8C%E5%96%B6%3F%E4%BB%95%E4%BA%8B%E5%A0%B4%3Dbusiness%26ID%3D1", "%2F%E7%B5%8C%E5%96%B6%3F%E4%BB%95%E4%BA%8B%E5%A0%B4%3Dbusiness%26ID%3D1")]
        public void Should_Return_Relative_Url(string url, string expected)
        {
            LocalIocManager.Resolve<IUrlHelper>().LocalPathAndQuery(url).ShouldBe(expected);
        }

        [Theory]
        [InlineData("invalid relative path")]
        [InlineData("/invalid relative path")]
        [InlineData("/\\invalid/relative/path")]
        public void Should_Not_Return_Relative_Url(string url)
        {
            LocalIocManager.Resolve<IUrlHelper>().LocalPathAndQuery(url).ShouldBeNull();
        }

        [Theory]
        [InlineData("www.example.com", "/")]
        [InlineData("www.example.com/site", "/site")]
        [InlineData("www.example.com/site?id=1&value=2", "/site?id=1&value=2")]
        [InlineData("http://www.example.com", "/")]
        [InlineData("http://www.example.com/site", "/site")]
        [InlineData("http://www.example.com/site?id=1&value=2", "/site?id=1&value=2")]
        [InlineData("http://www.example.com:80", "/")]
        [InlineData("http://www.example.com:80/site", "/site")]
        [InlineData("http://www.example.com:80/site?id=1&value=2", "/site?id=1&value=2")]
        [InlineData("http%3A%2F%2Fwww.example.com%2F%E7%B5%8C%E5%96%B6%3F%E4%BB%95%E4%BA%8B%E5%A0%B4%3Dbusiness%26ID%3D1", "http%3A%2F%2Fwww.example.com%2F%E7%B5%8C%E5%96%B6%3F%E4%BB%95%E4%BA%8B%E5%A0%B4%3Dbusiness%26ID%3D1")]
        public void Should_Return_Relative_Url_With_Host(string url, string expected)
        {
            LocalIocManager.Resolve<IUrlHelper>().LocalPathAndQuery(url, "www.example.com", null).ShouldBe(expected);
            LocalIocManager.Resolve<IUrlHelper>().LocalPathAndQuery(url, "www.example.com", 80).ShouldBe(expected);
        }

        [Theory]
        [InlineData("http://www.example.com:8080", "/")]
        [InlineData("http://www.example.com:8080/site", "/site")]
        [InlineData("http://www.example.com:8080/site?id=1&value=2", "/site?id=1&value=2")]
        public void Should_Return_Relative_Url_With_Host_And_Port(string url, string expected)
        {
            LocalIocManager.Resolve<IUrlHelper>().LocalPathAndQuery(url, "www.example.com", 8080).ShouldBe(expected);
        }

        [Theory]
        [InlineData("www.counterexample.com")]
        [InlineData("www.counterexample.com/site")]
        [InlineData("http://www.counterexample.com")]
        [InlineData("http://www.counterexample.com/site")]
        [InlineData("ftp://www.example.com")]
        public void Should_Not_Return_Relative_Url_With_Host(string url)
        {
            LocalIocManager.Resolve<IUrlHelper>().LocalPathAndQuery(url, "www.example.com", null).ShouldBeNull();
            LocalIocManager.Resolve<IUrlHelper>().LocalPathAndQuery(url, "www.example.com", 80).ShouldBeNull();
        }

        [Theory]
        [InlineData("www.example.com:8080")]
        [InlineData("www.example.com:8080/site")]
        [InlineData("http://www.example.com:8080")]
        [InlineData("http://www.example.com:8080/site")]
        [InlineData("ftp://www.example.com:21")]
        public void Should_Not_Return_Relative_Url_With_Host_And_Port(string url)
        {
            LocalIocManager.Resolve<IUrlHelper>().LocalPathAndQuery(url, "www.example.com", 8888).ShouldBeNull();
        }
    }
}
