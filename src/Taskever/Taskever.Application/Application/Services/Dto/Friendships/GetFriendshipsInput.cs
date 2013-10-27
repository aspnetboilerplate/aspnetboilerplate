using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Taskever.Domain.Enums;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class GetFriendshipsInput : IInputDto
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        public FriendshipStatus? Status { get; set; }

        public bool? CanAssignTask { get; set; }
    }
}