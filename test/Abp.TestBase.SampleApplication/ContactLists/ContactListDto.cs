using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Abp.TestBase.SampleApplication.ContactLists
{
    [AbpAutoMapFrom(typeof(ContactList))]
    public class ContactListDto : EntityDto
    {
        public virtual string Name { get; set; }
    }
}