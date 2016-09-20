using System;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class Repository_Resolve_Tests : EntityFrameworkCoreModuleTestBase
    {
        [Fact]
        public void Should_Resolve_Custom_Repository_If_Registered()
        {
            var postRepository = Resolve<IRepository<Post, Guid>>();

            postRepository.GetAllList().Any().ShouldBeTrue();

            Assert.Throws<ApplicationException>(
                () => postRepository.Count()
            ).Message.ShouldBe("can not get count of posts");

            //Should also resolve by custom interface and implementation
            Resolve<IPostRepository>();
            Resolve<PostRepository>();
        }
    }
}