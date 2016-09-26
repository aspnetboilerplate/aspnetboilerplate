using Abp.Application.Services.Dto;

namespace Abp.TestBase.SampleApplication.Messages
{
    public class GetMessagesWithFilterInput : PagedAndSortedResultRequestDto
    {
        public string Text { get; set; }
    }
}