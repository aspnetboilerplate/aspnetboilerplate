using Abp.Domain.Repositories;
using Abp.Reflection;
using Abp.TestBase.SampleApplication.ContactLists;
using Abp.TestBase.SampleApplication.EntityFramework.Repositories;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Reflection
{
    public class ProxyHelper_Tests: SampleApplicationTestBase
    {
        private readonly IRepository<ContactList> _contactListRepository;

        public ProxyHelper_Tests()
        {
            _contactListRepository = Resolve<IRepository<ContactList>>();
        }

        [Fact]
        public void ProxyHelper_Should_Return_Original_Object_For_Not_Proxied_Object()
        {
            (ProxyHelper.UnProxy(new MyTestClass()) is MyTestClass).ShouldBeTrue();
        }

        [Fact]
        public void ProxyHelper_Should_Return_Unproxied_Object_For_Proxied_Object()
        {
            (ProxyHelper.UnProxy(_contactListRepository) is SampleApplicationEfRepositoryBase<ContactList>).ShouldBeTrue();
        }

        class MyTestClass
        {

        }
    }
}
