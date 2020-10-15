using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.TestBase.SampleApplication.Messages;
using JetBrains.Annotations;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Session
{
    public class Session_Tests : SampleApplicationTestBase
    {
        private readonly IAbpSession _session;
        private IRepository<Message> _messageRepository;
        private IUnitOfWorkManager _unitOfWorkManager;

        public Session_Tests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            _session = Resolve<IAbpSession>();

            _messageRepository = Resolve<IRepository<Message>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void Session_Override_Test()
        {
            _session.UserId.ShouldBe(null);
            _session.TenantId.ShouldBe(null);

            using (_session.Use(42, 571))
            {
                _session.TenantId.ShouldBe(42);
                _session.UserId.ShouldBe(571);

                using (_session.Use(null, 3))
                {
                    _session.TenantId.ShouldBe(null);
                    _session.UserId.ShouldBe(3);
                }

                _session.TenantId.ShouldBe(42);
                _session.UserId.ShouldBe(571);
            }

            _session.UserId.ShouldBe(null);
            _session.TenantId.ShouldBe(null);
        }

        [Fact]
        public async Task Should_Uow_Set_Tenant_Null_If_Completed_Outside_Session_Use_Using()
        {
            AbpSession.TenantId = 1;

            var messageEntity = new Message() { Text = "Test Message" };

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_session.Use(null, 3))
                {
                    await _messageRepository.InsertAsync(messageEntity);
                }
                await uow.CompleteAsync();
            }

            UsingDbContext(context =>
            {
                var entity = context.Messages.Single(x => x.Id == messageEntity.Id);
                entity.Text.ShouldBe(messageEntity.Text);
                entity.TenantId.ShouldBe(null);
            });
        }

        [Fact]
        public async Task Should_Uow_Set_Tenant_Null_If_Completed_Inside_Session_Use_Using()
        {
            AbpSession.TenantId = 1;

            var messageEntity = new Message() { Text = "Test Message" };

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_session.Use(null, 3))
                {
                    await _messageRepository.InsertAsync(messageEntity);
                    await uow.CompleteAsync();
                }
            }

            UsingDbContext(context =>
            {
                var entity = context.Messages.Single(x => x.Id == messageEntity.Id);
                entity.Text.ShouldBe(messageEntity.Text);
                entity.TenantId.ShouldBe(null);
            });
        }
    }
}
