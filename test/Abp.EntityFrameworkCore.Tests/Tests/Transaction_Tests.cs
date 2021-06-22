using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class Transaction_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IRepository<Blog> _blogRepository;

        public Transaction_Tests()
        {
            _uowManager = Resolve<IUnitOfWorkManager>();
            _blogRepository = Resolve<IRepository<Blog>>();
        }

        [Fact] 
        public async Task Should_Rollback_Transaction_On_Failure()
        {
            const string exceptionMessage = "This is a test exception!";

            var blogName = Guid.NewGuid().ToString("N");

            try
            {
                using (var uow = _uowManager.Begin())
                {
                    await _blogRepository.InsertAsync(
                        new Blog(blogName, $"http://{blogName}.com/")
                        );

                    throw new Exception(exceptionMessage); //Rollbacks transaction.
                    
                    // Unreachable code.
                    // await uow.CompleteAsync();
                }
            }
            catch (Exception ex) when (ex.Message == exceptionMessage)
            {

            }
            
            await UsingDbContextAsync(async context =>
            {
                var blog = await context.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
                blog.ShouldBeNull();
            });
        }
    }
}
