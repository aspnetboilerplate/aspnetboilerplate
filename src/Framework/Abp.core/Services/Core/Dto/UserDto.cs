namespace Abp.Services.Core.Dto
{
    /// <summary>
    /// Simple User DTO class.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Id of the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string EmailAddress { get; set; }
    }
}
