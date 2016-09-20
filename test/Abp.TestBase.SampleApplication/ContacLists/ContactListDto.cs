using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Abp.TestBase.SampleApplication.ContacLists
{
    [AutoMapFrom(typeof(ContactList))]
    public class ContactListDto : EntityDto
    {
        public virtual string Name { get; set; }
    }
}