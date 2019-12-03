
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Extensions;
using Abp.EntityFrameworkCore.Repositories;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.Zero.EntityChangeTracker
{
    public class EntityChangeTracker_Test : AbpZeroTestBase
    {
        private readonly IRepository<Blog> _blogRepository;

        public EntityChangeTracker_Test()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
        }

        [Fact]
        public void Entity_Change_Should_Check_OwnedEntity()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");

                //blog1.More is Owned Entity
                blog1.More.BloggerName = "test-blog-2";

                _blogRepository.GetDbContext().Entry(blog1).State.ShouldBe(EntityState.Unchanged);
                _blogRepository.GetDbContext().Entry(blog1.More).State.ShouldBe(EntityState.Modified);
                _blogRepository.GetDbContext().Entry(blog1).CheckOwnedEntityChange().ShouldBeTrue();

                uow.Complete();
            }

        }
    }

}
