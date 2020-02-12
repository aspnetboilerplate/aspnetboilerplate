using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using Abp.TestBase.SampleApplication.ContactLists;
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
        [Fact]
        public async Task Should_Insert_New_Person()
        {
            var personAppService = Resolve<IPersonAppService>();

            ContactList contactList = null;
            int peopleCount = 0;

            await UsingDbContext(
                async context =>
                {
                    contactList = await context.ContactLists.FirstOrDefaultAsync();
                    peopleCount = await context.People.CountAsync();
                });

            await personAppService.CreatePersonAsync(
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
            var personAppService = Resolve<IPersonAppService>();

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
                await personAppService.CreatePersonAsync(new CreatePersonInput { ContactListId = contactList.Id, Name = "john" });
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
            var personAppService = Resolve<IPersonAppService>();

            await Assert.ThrowsAsync<AbpValidationException>(async () => await personAppService.CreatePersonAsync(new CreatePersonInput { Name = null }));
        }

        [Fact]
        public void Should_Get_All_People_Without_Filter()
        {
            var personAppService = Resolve<IPersonAppService>();

            var output = personAppService.GetPeople(new GetPeopleInput());
            output.Items.Count.ShouldBe(UsingDbContext(context => context.People.Count(p => !p.IsDeleted)));
            output.Items.FirstOrDefault(p => p.Name == "halil").ShouldNotBe(null);
        }

        [Fact]
        public void Should_Get_Related_People_With_Filter()
        {
            var personAppService = Resolve<IPersonAppService>();

            var output = personAppService.GetPeople(new GetPeopleInput { NameFilter = "h" });
            output.Items.FirstOrDefault(p => p.Name == "halil").ShouldNotBe(null);
            output.Items.All(p => p.Name.Contains("h")).ShouldBe(true);
        }

        [Fact]
        public async Task Should_Delete_Person()
        {
            //Arrange

            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync("CanDeletePerson")
                .Returns(
                    async info =>
                    {
                        await Task.Delay(10);
                        return true;
                    });
            permissionChecker.IsGranted("CanDeletePerson").Returns(true);

            LocalIocManager.IocContainer.Register(
                Component.For<IPermissionChecker>().Instance(permissionChecker).IsDefault()
            );

            var personAppService = Resolve<IPersonAppService>();

            AbpSession.UserId = 1;

            var halil = await UsingDbContextAsync(async context => await context.People.SingleAsync(p => p.Name == "halil"));

            //Act

            await personAppService.DeletePerson(new EntityDto(halil.Id));

            //Assert

            (await UsingDbContextAsync(async context => await context.People.FirstOrDefaultAsync(p => p.Name == "halil"))).IsDeleted.ShouldBe(true);
        }

        [Fact]
        public async Task Should_Not_Delete_Person_If_UnAuthorized()
        {
            //Arrange

            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync("CanDeletePerson")
                .Returns(async info =>
                {
                    await Task.Delay(10);
                    return false;
                });

            LocalIocManager.IocContainer.Register(
                Component.For<IPermissionChecker>().Instance(permissionChecker).IsDefault()
            );

            var personAppService = Resolve<IPersonAppService>();

            AbpSession.UserId = 1;
            
            var halil = await UsingDbContextAsync(async context => await context.People.SingleAsync(p => p.Name == "halil"));

            //Act & Assert

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await personAppService.DeletePerson(new EntityDto(halil.Id));
            });
        }

        [Fact]
        public void Test_TestPrimitiveMethod()
        {
            var personAppService = Resolve<IPersonAppService>();

            personAppService.TestPrimitiveMethod(42, "adana", new EntityDto(7)).ShouldBe("42#adana#7");
        }
    }
}
