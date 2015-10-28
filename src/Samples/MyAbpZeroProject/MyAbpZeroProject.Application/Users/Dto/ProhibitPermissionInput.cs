using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace MyAbpZeroProject.Users.Dto
{
    public class ProhibitPermissionInput : IInputDto
    {
        [Range(1, long.MaxValue)]
        public int UserId { get; set; }

        [Required]
        public string PermissionName { get; set; }
    }
}