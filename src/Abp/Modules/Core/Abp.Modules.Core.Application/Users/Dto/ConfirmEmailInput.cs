using System.ComponentModel.DataAnnotations;

namespace Abp.Users.Dto
{
    public class ConfirmEmailInput
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        public string ConfirmationCode { get; set; }
    }
}