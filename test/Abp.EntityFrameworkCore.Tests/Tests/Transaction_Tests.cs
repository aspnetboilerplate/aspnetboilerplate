using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    //WE CAN NOT TEST TRANSACTIONS SINCE INMEMORY DB DOES NOT SUPPORT IT! TODO: Use SQLite
    public class Transaction_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IRepository<Blog> _blogRepository;

        public Transaction_Tests()
        {
            _uowManager = Resolve<IUnitOfWorkManager>();
            _blogRepository = Resolve<IRepository<Blog>>();
        }

        //[Fact] 
        public async Task Should_Rollback_Transaction_On_Failure()
        {
            const string exceptionMessage = "This is a test exception!";

            var blogName = Guid.NewGuid().ToString("N");

            try
            {
                using (_uowManager.Begin())
                {
                    await _blogRepository.InsertAsync(
                        new Blog(blogName, $"http://{blogName}.com/")
                        );

                    throw new ApplicationException(exceptionMessage);
                }
            }
            catch (ApplicationException ex) when (ex.Message == exceptionMessage)
            {

            }
            
            await UsingDbContextAsync(async context =>
            {
                var blog = await context.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
                blog.ShouldNotBeNull();
            });
        }
    }
}