using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Taskever.Friendships.Dto
{
    public class GetFriendshipsInput : IInputDto
    {
        [Range(1, int.MaxValue)]
        public long UserId { get; set; }

        public FriendshipStatus? Status { get; set; }

        public bool? CanAssignTask { get; set; }
    }
}