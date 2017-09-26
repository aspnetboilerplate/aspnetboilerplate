using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.EntityFramework;
using Abp.TestBase.SampleApplication.People;
using EntityFramework.DynamicFilters;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class Two_DbContext_Uow_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<SecondDbContextEntity> _secondDbContextEntityRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public Two_DbContext_Uow_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();
            _secondDbContextEntityRepository = Resolve<IRepository<SecondDbContextEntity>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        //[Fact] //TODO: Not working since Effort does not support multiple db context!
        public async Task Should_Two_DbContext_Share_Same_Transaction()
        {
            var personInitial = UsingDbContext(context => context.People.Single(p => p.Name == "halil"));

            using (var uow = _unitOfWorkManager.Begin())
            {
                await _personRepository.UpdateAsync(new Person
                {
                    Id = personInitial.Id,
                    Name = "halil-updated",
                    ContactListId = personInitial.ContactListId
                });

                await _secondDbContextEntityRepository.InsertAsync(new SecondDbContextEntity {Name = "test1"});

                await uow.CompleteAsync();
            }

            var personFinal = UsingDbContext(context => context.People.Single(p => p.Id == personInitial.Id));
            personFinal.Name.ShouldBe("halil-updated");

            using (var secondContext = Resolve<SecondDbContext>())
            {
                secondContext.DisableAllFilters();
                secondContext.SecondDbContextEntities.Count(e => e.Name == "test1").ShouldBe(1);
            }
        }
    }
}