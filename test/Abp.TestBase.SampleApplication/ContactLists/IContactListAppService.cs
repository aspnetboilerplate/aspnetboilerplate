using System.Collections.Generic;
using Abp.Application.Services;

namespace Abp.TestBase.SampleApplication.ContactLists
{
    public interface IContactListAppService : IApplicationService
    {
        void Test();

        List<ContactListDto> GetContactLists();
    }
}
