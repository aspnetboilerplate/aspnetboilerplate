using System.ComponentModel.DataAnnotations;

namespace Abp.Zero.SampleApp.Users.Dto
{
    public class CreateUserInput
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public virtual string Surname { get; set; }

        [Required]
        public virtual string UserName { get; set; }

        [Required]
        public virtual string EmailAddress { get; set; }
        
        public int? TenantId { get; set; }
    }
}
