using System.Collections.Generic;
using Abp.Application.Features;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;

namespace Abp.TestBase.SampleApplication.ContacLists
{
    public class ContactListAppService : ApplicationService, IContactListAppService
    {
        private readonly IRepository<ContactList> _contactListRepository;

        public ContactListAppService(IRepository<ContactList> contactListRepository)
        {
            _contactListRepository = contactListRepository;
        }

        [RequiresFeature(SampleFeatureProvider.Names.Contacts)]
        public void Test()
        {
            //This method is called only if SampleFeatureProvider.Names.Contacts feature is enabled
        }

        public List<ContactListDto> GetContactLists()
        {
            return _contactListRepository.GetAllList().MapTo<List<ContactListDto>>();
        }
    }
}