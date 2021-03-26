using System;
using System.Threading.Tasks;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Dapper.Tests.Domain;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Dapper.Tests.Tests
{
    public class Transaction_Tests : AbpEfCoreDapperTestApplicationBase
    {
        private readonly IDapperRepository<Blog> _blogDapperRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IUnitOfWorkManager _uowManager;

        public Transaction_Tests()
        {
            _uowManager = Resolve<IUnitOfWorkManager>();
            _blogRepository = Resolve<IRepository<Blog>>();
            _blogDapperRepository = Resolve<IDapperRepository<Blog>>();
        }

        [Fact]
        public async Task Should_Rollback_Transaction_On_Failure()
        {
            const string exceptionMessage = "This is a test exception!";

            string blogName = Guid.NewGuid().ToString("N");

            try
            {
                using (_uowManager.Begin())
                {
                    await _blogRepository.InsertAsync(
                        new Blog(blogName, $"http://{blogName}.com/")
                    );

                    throw new Exception(exceptionMessage);
                }
            }
            catch (Exception ex) when (ex.Message == exceptionMessage)
            {
            }

            var blog = await _blogRepository.FirstOrDefaultAsync(x => x.Name == blogName);
            blog.ShouldBeNull();
        }

        [Fact]
        public void
            Dapper_and_EfCore_should_work_under_same_unitofwork_and_when_any_exception_appears_then_rollback_should_be_consistent_for_two_orm()
        {
            Resolve<IEventBus>().Register<EntityCreatingEventData<Blog>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Oguzhan_Same_Uow");

                    throw new Exception("Uow Rollback");
                });

            try
            {
                using (IUnitOfWorkCompleteHandle uow = Resolve<IUnitOfWorkManager>().Begin())
                {
                    var blogId = _blogDapperRepository.InsertAndGetId(
                        new Blog("Oguzhan_Same_Uow", "www.oguzhansoykan.com")
                    );

                    Blog person = _blogRepository.Get(blogId);

                    person.ShouldNotBeNull();

                    uow.Complete();
                }
            }
            catch (Exception exception)
            {
                //no handling.
            }

            _blogDapperRepository.FirstOrDefault(x => x.Name == "Oguzhan_Same_Uow").ShouldBeNull();
            _blogRepository.FirstOrDefault(x => x.Name == "Oguzhan_Same_Uow").ShouldBeNull();
        }

        [Fact]
        public async Task inline_sql_with_dapper_should_rollback_when_uow_fails()
        {
            Resolve<IEventBus>().Register<EntityCreatingEventData<Blog>>(eventData =>
            {
                eventData.Entity.Name.ShouldBe("Oguzhan_Same_Uow");
            });

            var blogId = 0;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                blogId = await _blogDapperRepository.InsertAndGetIdAsync(
                    new Blog("Oguzhan_Same_Uow", "www.aspnetboilerplate.com")
                );

                var person = await _blogRepository.GetAsync(blogId);

                person.ShouldNotBeNull();

                await uow.CompleteAsync();
            }

            try
            {
                using (IUnitOfWorkCompleteHandle uow = Resolve<IUnitOfWorkManager>()
                    .Begin(new UnitOfWorkOptions {IsTransactional = true}))
                {
                    await _blogDapperRepository.ExecuteAsync(
                        "Update Blogs Set Name = @name where Id =@id",
                        new
                        {
                            id = blogId, name = "Oguzhan_New_Blog"
                        }
                    );

                    throw new Exception("uow rollback");

                    await uow.CompleteAsync();
                }
            }
            catch (Exception exception)
            {
                //no handling.
            }

            (await _blogDapperRepository.FirstOrDefaultAsync(x => x.Name == "Oguzhan_New_Blog")).ShouldBeNull();
            (await _blogRepository.FirstOrDefaultAsync(x => x.Name == "Oguzhan_New_Blog")).ShouldBeNull();

            (await _blogDapperRepository.FirstOrDefaultAsync(x => x.Name == "Oguzhan_Same_Uow")).ShouldNotBeNull();
            (await _blogRepository.FirstOrDefaultAsync(x => x.Name == "Oguzhan_Same_Uow")).ShouldNotBeNull();
        }
    }
}
