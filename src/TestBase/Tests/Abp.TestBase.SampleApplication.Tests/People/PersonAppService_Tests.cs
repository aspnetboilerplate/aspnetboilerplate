using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using Abp.TestBase.SampleApplication.People;
using Abp.TestBase.SampleApplication.People.Dto;
using Abp.TestBase.SampleApplication.Tests.TestUtils;
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
            _personAppService = LocalIocManager.Resolve<IPersonAppService>();
        }

        private void InitializeData()
        {
            UsingDbContext(context => _initialPeople.ForEach(person => context.People.Add(person)));
        }

        [Fact]
        public async Task Should_Insert_New_Person()
        {
            await _personAppService.CreatePersonAsync(new CreatePersonInput { Name = "john" });

            UsingDbContext(async context =>
                {
                    (await context.People.CountAsync()).ShouldBe(_initialPeople.Count + 1);
                    (await context.People.FirstOrDefaultAsync(p => p.Name == "john")).ShouldNotBe(null);
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
    }
}
