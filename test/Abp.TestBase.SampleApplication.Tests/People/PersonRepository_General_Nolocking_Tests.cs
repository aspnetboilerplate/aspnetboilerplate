using System.Linq;

using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework.Extensions;
using Abp.TestBase.SampleApplication.People;

using Shouldly;

using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class PersonRepository_General_Nolocking_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PersonRepository_General_Nolocking_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        //[Fact(Skip = "Skipped, since Effort.DbConnection does not provide Sql Text while interception time.")]
        public void Should_Nolocking_Work()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                Person person = _personRepository.Nolocking(persons => persons.FirstOrDefault(x => x.Name == "halil"));
                person.ShouldNotBeNull();

                uow.Complete();
            }
        }
    }
}
