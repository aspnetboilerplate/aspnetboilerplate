using Abp.Extensions;
using Microsoft.AspNet.Identity;

namespace Abp.Zero.AspNetCore
{
    public class ExternalLoginUserInfo
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }

        public UserLoginInfo LoginInfo { get; set; }

        public bool HasAllNonEmpty()
        {
            return !Name.IsNullOrEmpty() &&
                   !Surname.IsNullOrEmpty() &&
                   !EmailAddress.IsNullOrEmpty();
        }
    }
}