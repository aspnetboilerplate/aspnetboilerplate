using Abp.Application.Services.Dto;

namespace Abp.TestBase.SampleApplication.Messages
{
    public class GetMessagesWithFilterInput : PagedAndSortedResultRequestInput
    {
        public string Text { get; set; }
    }
}