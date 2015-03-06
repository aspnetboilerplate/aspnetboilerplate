using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Validation;
using Abp.TestBase.SampleApplication.People;
using Abp.TestBase.SampleApplication.People.Dto;
using Abp.TestBase.SampleApplication.Tests.TestUtils;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class PersonAppService_Tests : SampleApplicationTestBase
    {
        private readonly IPersonAppService _personAppService;
        private readonly List<Person> _initialPeople = new List<Person>()
        {
            new Person {Name = "halil"},
            new Person {Name = "emre"}
        };

        public PersonAppService_Tests()
        {
            InitializeData();
            _personAppService = Resolve<IPersonAppService>();
        }

        private void InitializeData()
        {
            UsingDbContext(context => _initialPeople.ForEach(person => context.People.Add(person)));
        }

        [Fact]
        public async Task Should_Insert_New_Person()
        {
            await _personAppService.CreatePersonAsync(new CreatePersonInput { Name = "john" });

            await UsingDbContext(async context =>
            {
                (await context.People.CountAsync()).ShouldBe(_initialPeople.Count + 1);
                (await context.People.FirstOrDefaultAsync(p => p.Name == "john")).ShouldNotBe(null);
            });
        }

        [Fact]
        public async Task Should_Rollback_If_Uow_Is_Not_Completed()
        {
            //CreatePersonAsync will use same UOW.
            using (var uow = LocalIocManager.Resolve<IUnitOfWorkManager>().Begin())
            {
                await _personAppService.CreatePersonAsync(new CreatePersonInput { Name = "john" });
                //await uow.CompleteAsync(); //It's intentionally removed from code to see roll-back
            }

            //john will not be added since uow is not completed (so, rolled back)
            await UsingDbContext(async context =>
            {
                (await context.People.CountAsync()).ShouldBe(_initialPeople.Count);
                (await context.People.FirstOrDefaultAsync(p => p.Name == "john")).ShouldBe(null);
            });
        }

        [Fact]
        public async Task Should_Not_Insert_For_Invalid_Input()
        {
            await AssertEx.ThrowsAsync<AbpValidationException>(async () => await _personAppService.CreatePersonAsync(new CreatePersonInput { Name = null }));
        }

        [Fact]
        public void Should_Get_All_People_Without_Filter()
        {
            var output = _personAppService.GetPeople(new GetPeopleInput());
            output.Items.Count.ShouldBe(_initialPeople.Count);
            output.Items.FirstOrDefault(p => p.Name == "halil").ShouldNotBe(null);
        }

        [Fact]
        public void Should_Get_Related_People_With_Filter()
        {
            var output = _personAppService.GetPeople(new GetPeopleInput { NameFilter = "e" });
            output.Items.FirstOrDefault(p => p.Name == "emre").ShouldNotBe(null);
            output.Items.All(p => p.Name.Contains("e")).ShouldBe(true);
        }

        //[Fact] //Causes bug #345
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
            await _personAppService.DeletePerson(new EntityRequestInput(halil.Id));
            (await UsingDbContextAsync(async context => await context.People.FirstOrDefaultAsync(p => p.Name == "halil"))).ShouldBe(null);
        }
    }
}
