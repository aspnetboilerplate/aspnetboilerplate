using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework.GraphDiff.Extensions;
using Abp.TestBase.SampleApplication.ContactLists;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.EntityFramework.GraphDiff
{
    public class GraphDiffExtensions_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<ContactList> _contactListRepository;
        private readonly IRepository<Person> _peopleRepository;

        public GraphDiffExtensions_Tests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            AbpSession.TenantId = 3;

            _contactListRepository = Resolve<IRepository<ContactList>>();
            _peopleRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void New_Entity_Should_Be_Added_With_Navigation_Properties()
        {
            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                var list1 = _contactListRepository.FirstOrDefault(list => list.Name == "List-1 of Tenant-3");

                var davidDoe = new Person { Name = "David Doe", ContactList = new ContactList {Id = list1.Id} };
                davidDoe = _peopleRepository.AttachGraph(davidDoe);
                unitOfWorkManager.Current.SaveChanges();

                davidDoe.Id.ShouldNotBeNull();
                davidDoe.Id.ShouldNotBe(default(int));
                davidDoe.ContactListId.ShouldBe(list1.Id); //New entity should be attached with it's navigation property

                unitOfWork.Complete();
            }
        }

        [Fact]
        public void Disattached_Entity_Can_Be_Attached_To_Update_Corresponding_Existing_Entity_With_Navigation_Properties()
        {
            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                var list1 = _contactListRepository.FirstOrDefault(list => list.Name == "List-1 of Tenant-3");
                var list2 = _contactListRepository.FirstOrDefault(list => list.Name == "List-2 of Tenant-3");
                var johnDoeFromContext = _peopleRepository.FirstOrDefault(p => p.Name == "John Doe");

                unitOfWorkManager.Current.SaveChanges();

                johnDoeFromContext.ContactListId.ShouldBe(list1.Id); //Ensure that johnDoe is in list1 now

                var johnDoeDisattached = new Person {
                    Id = johnDoeFromContext.Id,
                    Name = "John Doe Junior",
                    ContactList = new ContactList { Id = list2.Id }
                };

                //As a result of graph attachment, we should get old entity with UPDATED nav property (EF6 would create a new entity as it's disattached);
                var johnDoeAfterBeeingAttached = _peopleRepository.AttachGraph(johnDoeDisattached);
                unitOfWorkManager.Current.SaveChanges();

                johnDoeAfterBeeingAttached.Id.ShouldBe(johnDoeFromContext.Id); //As entity was detached (but not deleted), it should be updated (not re-created)
                johnDoeAfterBeeingAttached.Name.ShouldBe("John Doe Junior"); //As entity was detached (but not deleted), it should be updated (not re-created)
                johnDoeAfterBeeingAttached.ContactListId.ShouldBe(list2.Id); //Entity should be attached with it's navigation property

                unitOfWork.Complete();
            }
        }
    }
}
