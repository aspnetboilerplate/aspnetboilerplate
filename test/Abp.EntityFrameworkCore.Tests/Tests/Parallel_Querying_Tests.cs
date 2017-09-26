using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class Parallel_Querying_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly ParallelQueryExecuteDemo _parallelQueryExecuteDemo;

        public Parallel_Querying_Tests()
        {
            _parallelQueryExecuteDemo = Resolve<ParallelQueryExecuteDemo>();
        }

        //[Fact]
        public async Task Should_Run_Parallel_With_Different_UnitOfWorks()
        {
            await _parallelQueryExecuteDemo.RunAsync();
        }
    }

    public class ParallelQueryExecuteDemo : ITransientDependency
    {
        private readonly IRepository<Blog> _blogRepository;

        public ParallelQueryExecuteDemo(IRepository<Blog> blogRepository)
        {
            _blogRepository = blogRepository;
        }

        [UnitOfWork]
        public virtual async Task RunAsync()
        {
            const int threadCount = 32;

            var tasks = new List<Task<int>>();

            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(GetBlogCountAsync());
            }

            await Task.WhenAll(tasks.Cast<Task>().ToArray());

            foreach (var task in tasks)
            {
                task.Result.ShouldBeGreaterThan(0);
            }
        }

        [UnitOfWork(TransactionScopeOption.RequiresNew, false)]
        public virtual async Task<int> GetBlogCountAsync()
        {
            await Task.Delay(RandomHelper.GetRandom(0, 100));
            var result = await _blogRepository.GetAll().CountAsync();
            await Task.Delay(RandomHelper.GetRandom(0, 100));
            return result;
        }
    }
}
