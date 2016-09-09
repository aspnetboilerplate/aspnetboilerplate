using System;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Abp.TestBase.SampleApplication.Messages;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Messages
{
    public class MessageAppService_Tests : SampleApplicationTestBase
    {
        private readonly MessageAppService _messageAppService;

        public MessageAppService_Tests()
        {
            _messageAppService = Resolve<MessageAppService>();
        }

        protected override void CreateInitialData()
        {
            UsingDbContext(
                context =>
                {
                    context.Messages.Add(
                        new Message
                        {
                            TenantId = 1,
                            Text = "tenant-1-message-1"
                        });

                    context.Messages.Add(
                        new Message
                        {
                            TenantId = 1,
                            Text = "tenant-1-message-2"
                        });
                });
        }

        [Fact]
        public void Should_Get_All_Messages()
        {
            var messages = _messageAppService.GetAll(new GetMessagesWithFilterInput());
            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(2);
        }

        [Fact]
        public void Should_Get_All_Messages_With_Filtering()
        {
            var messages = _messageAppService.GetAll(new GetMessagesWithFilterInput { Text = "message-1" });
            messages.TotalCount.ShouldBe(1);
            messages.Items.Count.ShouldBe(1);
            messages.Items[0].Text.ShouldBe("tenant-1-message-1");
        }

        [Fact]
        public void Should_Get_Message()
        {
            var message = _messageAppService.Get(new IdInput(2));
            message.Text.ShouldBe("tenant-1-message-2");
        }

        [Fact]
        public void Should_Delete_Message()
        {
            UsingDbContext(context =>
            {
                context.Messages.FirstOrDefault(m => m.Text == "tenant-1-message-2").ShouldNotBeNull();
            });

            _messageAppService.Delete(new IdInput(2));

            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Text == "tenant-1-message-2").IsDeleted.ShouldBeTrue();
            });
        }

        [Fact]
        public void Should_Update_Message()
        {
            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Id == 2).Text.ShouldBe("tenant-1-message-2");
            });

            var message = _messageAppService.Get(new IdInput(2));

            message.Text = "tenant-1-message-2-updated";

            _messageAppService.Update(message);

            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Id == 2).Text.ShouldBe("tenant-1-message-2-updated");
            });
        }


        [Fact]
        public void Should_Create_Message()
        {
            var messageText = Guid.NewGuid().ToString("N");

            _messageAppService.Create(new MessageDto
            {
                Text = messageText
            });

            UsingDbContext(context =>
            {
                context.Messages.FirstOrDefault(m => m.Text == messageText).ShouldNotBeNull();
            });
        }
    }
}
