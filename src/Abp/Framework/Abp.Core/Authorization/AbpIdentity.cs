using System.Security.Principal;

namespace Abp.Authorization
{
    public class AbpIdentity : IIdentity
    {
        public int UserId { get; private set; }

        public string Name { get; private set; }

        public string ShortName { get; private set; }

        public string FullName { get; private set; }

        public string EmailAddress { get; private set; }

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated { get; private set; }

        private AbpIdentity()
        {
            AuthenticationType = "abp";
            IsAuthenticated = false;
        }

        public AbpIdentity(int userId, string name, string shortName, string fullName, string emailAddress)
            : this()
        {
            UserId = userId;
            Name = name;
            ShortName = shortName;
            FullName = fullName;
            EmailAddress = emailAddress;
        }
    }
}