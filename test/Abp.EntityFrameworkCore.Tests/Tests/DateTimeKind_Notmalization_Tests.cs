using System;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class DateTimeKind_Notmalization_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<BlogCategory> _blogCategoryRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DateTimeKind_Notmalization_Tests()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
            _blogCategoryRepository = Resolve<IRepository<BlogCategory>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void DateTime_Kind_Should_Be_Normalized_To_UTC_Test()
        {
            // Clock.Provider is set to 'ClockProviders.Utc' 
            // in the constructor of EntityFrameworkCoreModuleTestBase
            Clock.Kind.ShouldBe(DateTimeKind.Utc);

            //Act

            var blogs = _blogRepository.GetAllList();

            //Assert

            blogs.Count.ShouldBeGreaterThan(0);

            foreach (var blog in blogs)
            {
                blog.CreationTime.Kind.ShouldBe(DateTimeKind.Utc);
                blog.DeletionTime.ShouldNotBeNull();
                blog.DeletionTime.Value.Kind.ShouldBe(DateTimeKind.Utc);
                blog.DeletionTime.Value.ToString("yyy-MM-dd HH:mm:ss").ShouldBe("2019-01-01 00:00:00");
                blog.BlogTime.LastAccessTime.Kind.ShouldBe(DateTimeKind.Utc);
                blog.BlogTime.LatestPosTime.Kind.ShouldNotBe(DateTimeKind.Utc);
            }
        }

        [Fact]
        public void DateTime_Kind_Should_Not_Be_Normalized_Test()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var category = _blogCategoryRepository.GetAllList().FirstOrDefault();
                _blogCategoryRepository.EnsureCollectionLoaded(category, c => c.SubCategories);

                //Assert

                category.ShouldNotBeNull();
                category.CreationTime.Kind.ShouldBe(DateTimeKind.Unspecified);

                foreach (var subCategory in category.SubCategories)
                {
                    subCategory.CreationTime.Kind.ShouldBe(DateTimeKind.Unspecified);
                }

                uow.Complete();
            }
        }
    }
}
