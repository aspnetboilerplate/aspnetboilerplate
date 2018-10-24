using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class PersonRepository_SoftDelete_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> _personRepository;

        public PersonRepository_SoftDelete_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void Should_Not_Retrieve_Soft_Deleteds_As_Default()
        {
            _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false);
        }

        [Fact]
        public void Should_Retrive_Soft_Deleteds_If_Filter_Is_Disabled()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();
            using (var ouw = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(true); //Getting deleted people
                }

                ouw.Complete();
            }
        }

        [Fact]
        public void Should_Disable_And_Enable_Filters_For_SoftDelete()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();
            using (var ouw = uowManager.Begin())
            {
                _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people since soft-delete is enabled by default

                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(true); //getting deleted people

                    using (uowManager.Current.EnableFilter(AbpDataFilters.SoftDelete)) //re-enabling filter
                    {
                        _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people

                        using (uowManager.Current.EnableFilter(AbpDataFilters.SoftDelete)) //enabling filter has no effect since it's already enabed
                        {
                            _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people
                        }

                        _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people
                    }

                    _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(true); //getting deleted people
                }

                _personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people

                ouw.Complete();
            }
        }

        [Fact]
        public async Task Should_Set_Deletion_Audit_Informations()
        {
            const long userId = 42;
            AbpSession.UserId = userId;

            var uowManager = Resolve<IUnitOfWorkManager>();

            //Get an entity to delete
            var personToBeDeleted = (await _personRepository.GetAllListAsync()).FirstOrDefault();
            personToBeDeleted.ShouldNotBe(null);

            //Deletion audit properties should be null since it's not deleted yet
            personToBeDeleted.IsDeleted.ShouldBe(false);
            personToBeDeleted.DeletionTime.ShouldBe(null);
            personToBeDeleted.DeleterUserId.ShouldBe(null);

            //Delete it
            await _personRepository.DeleteAsync(personToBeDeleted.Id);

            //Check if it's deleted
            (await _personRepository.FirstOrDefaultAsync(personToBeDeleted.Id)).ShouldBe(null);

            //Get deleted entity again and check audit informations
            using (var ouw = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    personToBeDeleted = await _personRepository.FirstOrDefaultAsync(personToBeDeleted.Id);
                    personToBeDeleted.ShouldNotBe(null);

                    //Deletion audit properties should be set
                    personToBeDeleted.IsDeleted.ShouldBe(true);
                    personToBeDeleted.DeletionTime.ShouldNotBe(null);
                    personToBeDeleted.DeleterUserId.ShouldBe(userId);
                }

                ouw.Complete();
            }
        }

        [Fact]
        public async Task Should_Permanently_Delete_SoftDelete_Entity_With_HarDelete_Method()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin())
            {
                var people = _personRepository.GetAllList();

                foreach (var person in people)
                {
                    await _personRepository.HardDeleteAsync(person);
                }

                uow.Complete();
            }

            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var poeple = _personRepository.GetAllList();
                    poeple.Count.ShouldBe(1);
                    poeple.First().Name.ShouldBe("emre");
                }

                uow.Complete();
            }
        }
    }
}
