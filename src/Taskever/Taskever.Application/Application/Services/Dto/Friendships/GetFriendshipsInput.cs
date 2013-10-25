using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Friendships
{
    [DataContract]
    public class GetFriendshipsInput : IInputDto
    {
        [Required]
        [DataMember(IsRequired = true)] //TODO: What is this?
        public int UserId { get; set; }

        public bool? CanAssignTask { get; set; }
    }
}