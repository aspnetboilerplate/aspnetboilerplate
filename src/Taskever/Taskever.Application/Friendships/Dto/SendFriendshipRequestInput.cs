using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Taskever.Friendships.Dto
{
    public class SendFriendshipRequestInput : IInputDto
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}