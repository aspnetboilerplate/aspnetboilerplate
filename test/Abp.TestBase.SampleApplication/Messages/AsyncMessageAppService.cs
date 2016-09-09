using Abp.Application.Services;
using Abp.Domain.Repositories;

namespace Abp.TestBase.SampleApplication.Messages
{
    public class AsyncMessageAppService : AsyncCrudAppService<Message, MessageDto>
    {
        public AsyncMessageAppService(IRepository<Message, int> repository)
            : base(repository)
        {

        }
    }
}