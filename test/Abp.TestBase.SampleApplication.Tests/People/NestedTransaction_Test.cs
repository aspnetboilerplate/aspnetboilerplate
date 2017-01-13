using System;
using System.Linq;
using System.Transactions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class NestedTransaction_Test : SampleApplicationTestBase
    {
        private readonly IRepository<Person> _personRepository;

        public NestedTransaction_Test()
        {
            _personRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void Should_Suppress_Outer_Transaction()
        {
            var outerUowPersonName = Guid.NewGuid().ToString("N");
            var innerUowPersonName = Guid.NewGuid().ToString("N");

            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            Assert.Throws<ApplicationException>(() =>
            {
                using (var uow = unitOfWorkManager.Begin())
                {
                    var contactList = UsingDbContext(context => context.ContactLists.First());

                    _personRepository.Insert(new Person
                    {
                        Name = outerUowPersonName,
                        ContactListId = contactList.Id
                    });

                    using (var innerUow = unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
                    {
                        _personRepository.Insert(new Person
                        {
                            Name = innerUowPersonName,
                            ContactListId = contactList.Id
                        });

                        innerUow.Complete();
                    }

                    throw new ApplicationException("This exception is thown to rollback outer transaction!");
                }

                return;
            }).Message.ShouldBe("This exception is thown to rollback outer transaction!");

            UsingDbContext(context =>
            {
                context.People.FirstOrDefault(n => n.Name == outerUowPersonName).ShouldBeNull();
                context.People.FirstOrDefault(n => n.Name == innerUowPersonName).ShouldNotBeNull();
            });
        }
    }
}
