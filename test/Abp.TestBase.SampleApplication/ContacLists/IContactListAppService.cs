using System.Collections.Generic;
using Abp.Application.Services;

namespace Abp.TestBase.SampleApplication.ContacLists
{
    public interface IContactListAppService : IApplicationService
    {
        void Test();

        List<ContactListDto> GetContactLists();
    }
}
