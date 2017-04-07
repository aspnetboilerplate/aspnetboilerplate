using Abp.Application.Services;

namespace Abp.TestBase.SampleApplication.Messages
{
    public interface IAsyncMessageAppService : IAsyncCrudAppService<MessageDto>
    {
        
    }
}