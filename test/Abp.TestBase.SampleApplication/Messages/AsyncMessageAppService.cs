using Abp.Application.Services;
using Abp.Domain.Repositories;

namespace Abp.TestBase.SampleApplication.Messages
{
    public class AsyncMessageAppService : AsyncCrudAppService<Message, MessageDto>, IAsyncMessageAppService
    {
        public AsyncMessageAppService(IRepository<Message> repository)
            : base(repository)
        {

        }
    }
}