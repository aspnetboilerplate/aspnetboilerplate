using Abp.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Web.Identity
{
    public class UserAdapter :IUser<int>
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public UserAdapter()
        {
            
        }

        public UserAdapter(User user)
        {
            Id = user.Id;
            UserName = user.EmailAddress;
        }
    }
}