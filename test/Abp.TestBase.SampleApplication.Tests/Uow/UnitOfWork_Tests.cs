using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.Messages;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Uow
{
    public class UnitOfWork_Tests : SampleApplicationTestBase
    {
        private IRepository<Message> _messageRepository;
        private IUnitOfWorkManager _unitOfWorkManager;
        private IMessageTestDomainService _messageTestDomainService;

        public UnitOfWork_Tests()
        {
            _messageRepository = Resolve<IRepository<Message>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _messageTestDomainService = Resolve<IMessageTestDomainService>();
        }

        [Fact]
        public async Task Should_Uow_Not_Set_Tenant_Null_If_Completed_Outside_SetTenantId_Using()
        {
            AbpSession.TenantId = 1;

            var messageEntity = new Message() { Text = "Test Message" };

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    await _messageRepository.InsertAsync(messageEntity);
                }
                await uow.CompleteAsync();
            }

            UsingDbContext(context =>
            {
                var entity = context.Messages.Single(x => x.Id == messageEntity.Id);
                entity.Text.ShouldBe(messageEntity.Text);
                entity.TenantId.ShouldBe(1);
            });
        }

        [Fact]
        public async Task Should_Uow_Set_Tenant_Null_If_Completed_Inside_SetTenantId_Using()
        {
            AbpSession.TenantId = 1;

            var messageEntity = new Message() { Text = "Test Message" };

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
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

        [Fact]
        public async Task Should_Uow_Not_Set_Tenant_Null_With_OUW_Attribute()
        {
            AbpSession.TenantId = 1;

            string newText = Guid.NewGuid().ToString("N");
            await _messageTestDomainService.InsetNewMessageWithUsingUOWSetTenantIdNull(newText);

            UsingDbContext(context =>
            {
                var entity = context.Messages.Single(x => x.Text == newText);
                entity.TenantId.ShouldBe(1);
            });
        }

        [Fact]
        public async Task Should_Uow_Set_Tenant_Null_With_OUW_Attribute_And_SaveChangesAsync()
        {
            AbpSession.TenantId = 1;

            string newText = Guid.NewGuid().ToString("N");
            await _messageTestDomainService.InsetNewMessageWithUsingUOWSetTenantIdNullAndSaveChangesAsync(newText);

            UsingDbContext(context =>
            {
                var entity = context.Messages.Single(x => x.Text == newText);
                entity.TenantId.ShouldBe(null);
            });
        }

    }
    public interface IMessageTestDomainService : IDomainService
    {
        Task InsetNewMessageWithUsingUOWSetTenantIdNull(string text);
        Task InsetNewMessageWithUsingUOWSetTenantIdNullAndSaveChangesAsync(string text);
    }

    public class MessageTestDomainService : DomainService, IMessageTestDomainService
    {
        private readonly IRepository<Message> _messageRepository;

        public MessageTestDomainService(IRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [UnitOfWork]
        public async Task InsetNewMessageWithUsingUOWSetTenantIdNull(string text)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                await _messageRepository.InsertAsync(new Message() { Text = text });
            }
        }

        [UnitOfWork]
        public async Task InsetNewMessageWithUsingUOWSetTenantIdNullAndSaveChangesAsync(string text)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                await _messageRepository.InsertAsync(new Message() { Text = text });
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}
