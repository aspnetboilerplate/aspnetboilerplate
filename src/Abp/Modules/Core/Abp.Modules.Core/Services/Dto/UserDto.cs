using Abp.Services.Dto;

namespace Abp.Modules.Core.Services.Dto
{
    /// <summary>
    /// Simple User DTO class.
    /// </summary>
    public class UserDto : EntityDto
    {
        /// <summary>
        /// Name of the user.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        public virtual string Surname { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public virtual string EmailAddress { get; set; }
    }
}
