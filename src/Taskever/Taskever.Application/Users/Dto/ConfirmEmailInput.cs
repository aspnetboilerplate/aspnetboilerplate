using System.ComponentModel.DataAnnotations;

namespace Abp.Users.Dto
{
    public class ConfirmEmailInput
    {
        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        [Required]
        public string ConfirmationCode { get; set; }
    }
}