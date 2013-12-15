using System.ComponentModel.DataAnnotations;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class ConfirmEmailInput
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        public string ConfirmationCode { get; set; }
    }
}