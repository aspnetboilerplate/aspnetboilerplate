using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class Repository_Filtering_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Post, Guid> _postRepository;

        public Repository_Filtering_Tests()
        {
            _postRepository = Resolve<IRepository<Post, Guid>>();
        }

        [Fact]
        public async Task Should_Filter_SoftDelete()
        {
            var posts = await _postRepository.GetAllListAsync();
            posts.All(p => !p.IsDeleted).ShouldBeTrue();
        }
    }
}
