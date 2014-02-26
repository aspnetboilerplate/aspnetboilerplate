using Abp.Security.Users;

namespace Taskever.Security.Users
{
    public class TaskeverUser : AbpUser
    {
        /// <summary>
        /// Profile image of the user. 
        /// </summary>
        public virtual string ProfileImage { get; set; }
    }
}
