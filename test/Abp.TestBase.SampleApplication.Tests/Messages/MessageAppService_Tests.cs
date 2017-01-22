using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.TestBase.SampleApplication.Messages;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Messages
{
    public class MessageAppService_Tests : SampleApplicationTestBase
    {
        private readonly MessageAppService _messageAppService;
        private readonly IAsyncMessageAppService _asyncMessageAppService;

        public MessageAppService_Tests()
        {
            _messageAppService = Resolve<MessageAppService>();
            _asyncMessageAppService = Resolve<IAsyncMessageAppService>();
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
            //Act

            var messages = _messageAppService.GetAll(new GetMessagesWithFilterInput());

            //Assert

            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(2);
        }

        [Fact]
        public void Should_Get_All_Messages_With_Filtering()
        {
            //Act

            var messages = _messageAppService.GetAll(new GetMessagesWithFilterInput { Text = "message-1" });

            //Assert

            messages.TotalCount.ShouldBe(1);
            messages.Items.Count.ShouldBe(1);
            messages.Items[0].Text.ShouldBe("tenant-1-message-1");
        }

        [Fact]
        public void Should_Get_All_Messages_With_Paging()
        {
            //Act

            var messages = _messageAppService.GetAll(new GetMessagesWithFilterInput { MaxResultCount = 1 });

            //Assert

            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(1);
            messages.Items[0].Text.ShouldBe("tenant-1-message-2");
        }

        [Fact]
        public void Should_Get_All_Messages_With_Paging_And_Sorting()
        {
            //Act

            var messages = _messageAppService.GetAll(new GetMessagesWithFilterInput { MaxResultCount = 1, Sorting = "Text" });

            //Assert

            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(1);
            messages.Items[0].Text.ShouldBe("tenant-1-message-1");
        }

        [Fact]
        public async Task Should_Get_All_Messages_With_Filtering_Async()
        {
            //Act

            var messages = await _asyncMessageAppService.GetAll(new PagedAndSortedResultRequestDto());

            //Assert

            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(2);
        }

        [Fact]
        public void Should_Get_Message()
        {
            //Act

            var message = _messageAppService.Get(new EntityDto(2));

            //Assert

            message.Text.ShouldBe("tenant-1-message-2");
        }

        [Fact]
        public void Should_Delete_Message()
        {
            //Arrange

            UsingDbContext(context =>
            {
                context.Messages.FirstOrDefault(m => m.Text == "tenant-1-message-2").ShouldNotBeNull();
            });

            //Act

            _messageAppService.Delete(new EntityDto(2));

            //Assert

            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Text == "tenant-1-message-2").IsDeleted.ShouldBeTrue();
            });
        }

        [Fact]
        public void Should_Update_Message()
        {
            //Arrange

            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Id == 2).Text.ShouldBe("tenant-1-message-2");
            });

            //Act

            var message = _messageAppService.Get(new EntityDto(2));
            message.Text = "tenant-1-message-2-updated";
            var updatedMessage = _messageAppService.Update(message);

            //Assert

            updatedMessage.Text.ShouldBe("tenant-1-message-2-updated");

            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Id == 2).Text.ShouldBe("tenant-1-message-2-updated");
            });
        }


        [Fact]
        public void Should_Create_Message()
        {
            //Arrange

            var messageText = Guid.NewGuid().ToString("N");

            //Act

            var createdMessage = _messageAppService.Create(new MessageDto { Text = messageText });

            //Assert

            createdMessage.Id.ShouldBeGreaterThan(0);
            createdMessage.Text.ShouldBe(messageText);

            UsingDbContext(context =>
            {
                context.Messages.FirstOrDefault(m => m.Text == messageText).ShouldNotBeNull();
            });
        }
    }
}
