using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using Abp.TestBase.SampleApplication.ContacLists;
using Abp.TestBase.SampleApplication.People;
using Abp.TestBase.SampleApplication.People.Dto;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class PersonAppService_Tests : SampleApplicationTestBase
    {
        private readonly IPersonAppService _personAppService;

        public PersonAppService_Tests()
        {
            _personAppService = Resolve<IPersonAppService>();
        }

        [Fact]
        public async Task Should_Insert_New_Person()
        {
            ContactList contactList = null;
            int peopleCount = 0;

            await UsingDbContext(
                async context =>
                {
                    contactList = await context.ContactLists.FirstOrDefaultAsync();
                    peopleCount = await context.People.CountAsync();
                });

            await _personAppService.CreatePersonAsync(
                new CreatePersonInput
                {
                    ContactListId = contactList.Id,
                    Name = "john"
                });

            await UsingDbContext(async context =>
            {
                (await context.People.FirstOrDefaultAsync(p => p.Name == "john")).ShouldNotBe(null);
                (await context.People.CountAsync()).ShouldBe(peopleCount + 1);
            });
        }

        [Fact]
        public async Task Should_Rollback_If_Uow_Is_Not_Completed()
        {
            ContactList contactList = null;
            int peopleCount = 0;

            await UsingDbContext(
                async context =>
                {
                    contactList = await context.ContactLists.FirstOrDefaultAsync();
                    peopleCount = await context.People.CountAsync();
                });

            //CreatePersonAsync will use same UOW.
            using (var uow = LocalIocManager.Resolve<IUnitOfWorkManager>().Begin())
            {
                await _personAppService.CreatePersonAsync(new CreatePersonInput { ContactListId = contactList.Id, Name = "john" });
                //await uow.CompleteAsync(); //It's intentionally removed from code to see roll-back
            }

            //john will not be added since uow is not completed (so, rolled back)
            await UsingDbContext(async context =>
            {
                (await context.People.FirstOrDefaultAsync(p => p.Name == "john")).ShouldBe(null);
                (await context.People.CountAsync()).ShouldBe(peopleCount);
            });
        }

        [Fact]
        public async Task Should_Not_Insert_For_Invalid_Input()
        {
            await Assert.ThrowsAsync<AbpValidationException>(async () => await _personAppService.CreatePersonAsync(new CreatePersonInput { Name = null }));
        }

        [Fact]
        public void Should_Get_All_People_Without_Filter()
        {
            var output = _personAppService.GetPeople(new GetPeopleInput());
            output.Items.Count.ShouldBe(UsingDbContext(context => context.People.Count(p => !p.IsDeleted)));
            output.Items.FirstOrDefault(p => p.Name == "halil").ShouldNotBe(null);
        }

        [Fact]
        public void Should_Get_Related_People_With_Filter()
        {
            var output = _personAppService.GetPeople(new GetPeopleInput { NameFilter = "h" });
            output.Items.FirstOrDefault(p => p.Name == "halil").ShouldNotBe(null);
            output.Items.All(p => p.Name.Contains("h")).ShouldBe(true);
        }

        [Fact]
        public async Task Should_Delete_Person()
        {
            AbpSession.UserId = 1;

            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync("CanDeletePerson").Returns(async info =>
                                                                        {
                                                                            await Task.Delay(10);
                                                                            return true;
                                                                        });

            LocalIocManager.IocContainer.Register(
                Component.For<IPermissionChecker>().UsingFactoryMethod(() => permissionChecker).LifestyleSingleton()
                );

            var halil = await UsingDbContextAsync(async context => await context.People.SingleAsync(p => p.Name == "halil"));
            await _personAppService.DeletePerson(new EntityDto(halil.Id));
            (await UsingDbContextAsync(async context => await context.People.FirstOrDefaultAsync(p => p.Name == "halil"))).IsDeleted.ShouldBe(true);
        }

        [Fact]
        public void Test_TestPrimitiveMethod()
        {
            _personAppService.TestPrimitiveMethod(42, "adana", new EntityDto(7)).ShouldBe("42#adana#7");
        }
    }
}
