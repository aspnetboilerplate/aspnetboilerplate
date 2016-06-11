using System.Data.Entity;
using System.Linq;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.ContactLists
{
    public class Querying_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Message> _messageRepository;

        public Querying_Tests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            _messageRepository = Resolve<IRepository<Message>>();
        }
        
        [Fact]
        public void Simple_Querying_With_AsNoTracking()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var messages = _messageRepository
                    .GetAll()
                    .AsNoTracking()
                    .ToList();

                messages.Any().ShouldBeTrue();

                uow.Complete();
            }
        }
    }
}