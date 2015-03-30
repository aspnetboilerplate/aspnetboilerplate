using System.Linq;
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
    }
}
