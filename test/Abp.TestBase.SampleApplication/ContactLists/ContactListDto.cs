using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Abp.TestBase.SampleApplication.ContactLists
{
    [AutoMapFrom(typeof(ContactList))]
    public class ContactListDto : EntityDto
    {
        public virtual string Name { get; set; }
    }
}